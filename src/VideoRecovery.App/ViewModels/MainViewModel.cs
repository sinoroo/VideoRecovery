using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using VideoRecovery.App.Services;
using VideoRecovery.Core.Abstractions.Recovery;

namespace VideoRecovery.App.ViewModels;

/// <summary>
/// 메인 화면 ViewModel. 폴더 선택 → 진단 시작 → 결과 표시 → 복구 실행(Phase 2)을 제어한다.
/// 통신/분석/복구 로직은 모두 서비스 인터페이스에 위임한다.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IFolderPickerService _folderPicker;
    private readonly IDiagnosisPipeline _pipeline;
    private readonly IRecoveryExecutor _recoveryExecutor;
    private readonly IFileSaveService _fileSave;
    private readonly ILogger<MainViewModel> _logger;

    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private string _title = "Video Recovery — 영상 진단/복구";

    [ObservableProperty]
    private string _selectedFolder = string.Empty;

    [ObservableProperty]
    private bool _recursive = true;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = "폴더를 선택하고 [분석 시작]을 누르세요.";

    [ObservableProperty]
    private int _progressValue;

    [ObservableProperty]
    private int _progressMax = 1;

    [ObservableProperty]
    private DiagnosisRowViewModel? _selectedRow;

    /// <summary>진단 결과 행 목록.</summary>
    public ObservableCollection<DiagnosisRowViewModel> Results { get; } = new();

    public MainViewModel(
        IFolderPickerService folderPicker,
        IDiagnosisPipeline pipeline,
        IRecoveryExecutor recoveryExecutor,
        IFileSaveService fileSave,
        ILogger<MainViewModel> logger)
    {
        _folderPicker = folderPicker;
        _pipeline = pipeline;
        _recoveryExecutor = recoveryExecutor;
        _fileSave = fileSave;
        _logger = logger;
    }

    /// <summary>진단 실행 가능 여부(폴더 선택됨 + 비실행 중).</summary>
    public bool CanAnalyze => !IsBusy && !string.IsNullOrWhiteSpace(SelectedFolder);

    /// <summary>선택 행 복구 가능 여부(비실행 중 + 복구 가능한 손상 행 선택).</summary>
    public bool CanRecoverSelected => !IsBusy && SelectedRow is { CanRecover: true };

    /// <summary>손상 전체 복구 가능 여부(비실행 중 + 복구 대상 존재).</summary>
    public bool CanRecoverAll => !IsBusy && Results.Any(r => r.CanRecover);

    /// <summary>선택 행에 저장 가능한 복구 결과물이 있는지 여부.</summary>
    public bool CanSaveRecovered => !IsBusy && SelectedRow is { IsRecovered: true };

    partial void OnIsBusyChanged(bool value)
    {
        AnalyzeCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
        RecoverSelectedCommand.NotifyCanExecuteChanged();
        RecoverAllDamagedCommand.NotifyCanExecuteChanged();
        SaveRecoveredCommand.NotifyCanExecuteChanged();
        OpenRecoveredFolderCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedFolderChanged(string value)
        => AnalyzeCommand.NotifyCanExecuteChanged();

    partial void OnSelectedRowChanged(DiagnosisRowViewModel? value)
    {
        RecoverSelectedCommand.NotifyCanExecuteChanged();
        SaveRecoveredCommand.NotifyCanExecuteChanged();
        OpenRecoveredFolderCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void BrowseFolder()
    {
        var folder = _folderPicker.PickFolder("진단할 HDD 또는 폴더 선택");
        if (!string.IsNullOrWhiteSpace(folder))
        {
            SelectedFolder = folder;
            StatusMessage = $"선택됨: {folder}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanAnalyze))]
    private async Task AnalyzeAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedFolder))
        {
            return;
        }

        IsBusy = true;
        Results.Clear();
        ProgressValue = 0;
        ProgressMax = 1;
        _cts = new CancellationTokenSource();

        var progress = new Progress<DiagnosisProgress>(OnProgress);

        try
        {
            StatusMessage = "분석 중...";
            var results = await _pipeline
                .RunAsync(SelectedFolder, Recursive, progress, _cts.Token)
                .ConfigureAwait(true);

            foreach (var result in results)
            {
                Results.Add(new DiagnosisRowViewModel(result));
            }

            var damaged = Results.Count(r => r.IsDamaged);
            StatusMessage = $"분석 완료 — 총 {Results.Count}개 영상 중 손상 {damaged}개 발견";
            _logger.LogInformation("분석 완료: 총 {Total}, 손상 {Damaged}", Results.Count, damaged);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "분석이 취소되었습니다.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "분석 중 오류");
            StatusMessage = $"오류: {ex.Message}";
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
            IsBusy = false;
            RecoverAllDamagedCommand.NotifyCanExecuteChanged();
            RecoverSelectedCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanRecoverSelected))]
    private async Task RecoverSelectedAsync()
    {
        if (SelectedRow is { } row)
        {
            IsBusy = true;
            _cts = new CancellationTokenSource();
            try
            {
                await RecoverRowAsync(row, _cts.Token).ConfigureAwait(true);
                StatusMessage = $"복구 완료 — {row.FileName}: {row.RecoveryStatus}";
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "복구가 취소되었습니다.";
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                IsBusy = false;
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanRecoverAll))]
    private async Task RecoverAllDamagedAsync()
    {
        var targets = Results.Where(r => r.CanRecover).ToList();
        if (targets.Count == 0)
        {
            return;
        }

        IsBusy = true;
        _cts = new CancellationTokenSource();
        var recovered = 0;

        try
        {
            var index = 0;
            foreach (var row in targets)
            {
                _cts.Token.ThrowIfCancellationRequested();
                index++;
                StatusMessage = $"복구 중... ({index}/{targets.Count}) {row.FileName}";
                var ok = await RecoverRowAsync(row, _cts.Token).ConfigureAwait(true);
                if (ok)
                {
                    recovered++;
                }
            }

            StatusMessage = $"복구 완료 — 대상 {targets.Count}개 중 {recovered}개 성공";
            _logger.LogInformation("전체 복구 완료: 대상 {Total}, 성공 {Recovered}", targets.Count, recovered);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = $"복구가 취소되었습니다 — {recovered}개 성공";
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
            IsBusy = false;
        }
    }

    /// <summary>단일 행을 복구하고 상태를 갱신한다. 성공 여부를 반환한다.</summary>
    private async Task<bool> RecoverRowAsync(DiagnosisRowViewModel row, CancellationToken cancellationToken)
    {
        if (row.Result is not { Health: { } before, Corruption: { } corruption, Plan: { } plan })
        {
            row.RecoveryStatus = "불필요";
            return false;
        }

        row.RecoveryStatus = "복구 중...";
        try
        {
            var outcome = await _recoveryExecutor
                .ExecuteAsync(row.Result.File.FullPath, before, corruption, plan, cancellationToken)
                .ConfigureAwait(true);

            row.RecoveryStatus = outcome.Succeeded ? "성공" : "실패";
            row.RecoveredPath = outcome.RecoveredPath ?? string.Empty;
            row.RecoveryDetail = outcome.Summary;
            _logger.LogInformation("복구 결과: {File} — {Summary}", row.FileName, outcome.Summary);

            if (ReferenceEquals(row, SelectedRow))
            {
                SaveRecoveredCommand.NotifyCanExecuteChanged();
                OpenRecoveredFolderCommand.NotifyCanExecuteChanged();
            }

            return outcome.Succeeded;
        }
        catch (OperationCanceledException)
        {
            row.RecoveryStatus = "취소됨";
            throw;
        }
        catch (Exception ex)
        {
            row.RecoveryStatus = "오류";
            row.RecoveryDetail = ex.Message;
            _logger.LogError(ex, "복구 중 오류: {File}", row.FileName);
            return false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveRecovered))]
    private void SaveRecovered()
    {
        if (SelectedRow is not { IsRecovered: true } row)
        {
            return;
        }

        try
        {
            // 원본 파일명 기준으로 저장명을 제안한다(확장자는 복구본을 따른다).
            var baseName = System.IO.Path.GetFileNameWithoutExtension(row.FileName);
            var extension = System.IO.Path.GetExtension(row.RecoveredPath);
            var suggested = $"{baseName}_recovered{extension}";

            var saved = _fileSave.SaveCopy(row.RecoveredPath, suggested);
            if (saved is not null)
            {
                StatusMessage = $"복구본 저장 완료 — {saved}";
                _logger.LogInformation("복구본 저장: {Source} → {Target}", row.RecoveredPath, saved);
            }
            else
            {
                StatusMessage = "저장이 취소되었습니다.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"저장 오류: {ex.Message}";
            _logger.LogError(ex, "복구본 저장 실패: {Path}", row.RecoveredPath);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveRecovered))]
    private void OpenRecoveredFolder()
    {
        if (SelectedRow is { IsRecovered: true } row)
        {
            _fileSave.RevealInExplorer(row.RecoveredPath);
        }
    }

    [RelayCommand(CanExecute = nameof(IsBusy))]
    private void Cancel() => _cts?.Cancel();

    [RelayCommand]
    private static void Exit() => Application.Current?.Shutdown();

    private void OnProgress(DiagnosisProgress p)
    {
        if (p.Total > 0)
        {
            ProgressMax = p.Total;
            ProgressValue = p.Completed;
        }

        var name = string.IsNullOrEmpty(p.CurrentFile)
            ? string.Empty
            : $" — {System.IO.Path.GetFileName(p.CurrentFile)}";
        StatusMessage = $"{p.Stage}{name} ({p.Completed}/{p.Total})";
    }
}
