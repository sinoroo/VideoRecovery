using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using VideoRecovery.App.Services;

namespace VideoRecovery.App.ViewModels;

/// <summary>
/// 메인 화면 ViewModel. 폴더 선택 → 진단 시작 → 결과 표시(MVP)를 제어한다.
/// 통신/분석 로직은 모두 서비스 인터페이스에 위임한다.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IFolderPickerService _folderPicker;
    private readonly IDiagnosisPipeline _pipeline;
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
        ILogger<MainViewModel> logger)
    {
        _folderPicker = folderPicker;
        _pipeline = pipeline;
        _logger = logger;
    }

    /// <summary>진단 실행 가능 여부(폴더 선택됨 + 비실행 중).</summary>
    public bool CanAnalyze => !IsBusy && !string.IsNullOrWhiteSpace(SelectedFolder);

    partial void OnIsBusyChanged(bool value)
    {
        AnalyzeCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedFolderChanged(string value)
        => AnalyzeCommand.NotifyCanExecuteChanged();

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
