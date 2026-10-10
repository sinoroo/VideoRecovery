using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.Scanner;

namespace VideoRecovery.App.ViewModels;

/// <summary>
/// MVP 메인 화면 ViewModel. 폴더 선택 → 스캔 → 분석 흐름을 조율한다. (요구사항 14, 15, 18)
/// 구현 클래스가 아닌 인터페이스에만 의존한다.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IVideoStorageScanner _scanner;
    private readonly IHealthAnalyzer _healthAnalyzer;
    private readonly ILogger<MainViewModel> _logger;

    public MainViewModel(
        IVideoStorageScanner scanner,
        IHealthAnalyzer healthAnalyzer,
        ILogger<MainViewModel> logger)
    {
        _scanner = scanner;
        _healthAnalyzer = healthAnalyzer;
        _logger = logger;
    }

    /// <summary>스캔된 영상 목록.</summary>
    public ObservableCollection<ScannedVideoViewModel> Videos { get; } = [];

    /// <summary>선택된 스캔 루트 폴더.</summary>
    [ObservableProperty]
    private string? _selectedFolder;

    /// <summary>진행 중 여부(버튼 비활성화 바인딩용).</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanAndAnalyzeCommand))]
    [NotifyCanExecuteChangedFor(nameof(SelectFolderCommand))]
    private bool _isBusy;

    /// <summary>진행률 (0.0 ~ 1.0).</summary>
    [ObservableProperty]
    private double _progress;

    /// <summary>상태 메시지.</summary>
    [ObservableProperty]
    private string _statusMessage = "복구할 HDD 또는 폴더를 선택하세요.";

    private bool CanRun => !IsBusy;

    /// <summary>폴더 선택 다이얼로그를 연다. (요구사항 18)</summary>
    [RelayCommand(CanExecute = nameof(CanRun))]
    private void SelectFolder()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "분석할 폴더 선택",
            Multiselect = false,
        };

        if (dialog.ShowDialog() == true)
        {
            SelectedFolder = dialog.FolderName;
            StatusMessage = $"선택됨: {SelectedFolder}";
            _logger.LogInformation("Folder selected: {Folder}", SelectedFolder);
        }
    }

    /// <summary>
    /// 선택된 폴더를 스캔하고 각 영상에 대해 건강 분석을 시도한다.
    /// 원본은 수정하지 않는다. (요구사항 2.1, 15)
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task ScanAndAnalyzeAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(SelectedFolder))
        {
            StatusMessage = "먼저 폴더를 선택하세요.";
            return;
        }

        IsBusy = true;
        Progress = 0;
        Videos.Clear();

        try
        {
            StatusMessage = "영상 스캔 중...";
            var scanProgress = new Progress<double>(p => Progress = p * 0.5);

            await foreach (var file in _scanner.ScanAsync(SelectedFolder, scanProgress, cancellationToken))
            {
                Videos.Add(new ScannedVideoViewModel(file));
            }

            _logger.LogInformation("Scan produced {Count} videos", Videos.Count);
            StatusMessage = $"{Videos.Count}개 영상 발견. 건강 분석 중...";

            // 건강 분석 단계 (현재 스텁 — Phase 1 에서 실제 구현).
            var index = 0;
            foreach (var video in Videos)
            {
                cancellationToken.ThrowIfCancellationRequested();
                video.Status = "분석 중";

                try
                {
                    video.Health = await _healthAnalyzer.RunAsync(video.Info, cancellationToken: cancellationToken);
                    video.Status = video.Health.Severity.ToString();
                }
                catch (NotImplementedException)
                {
                    video.Status = "분석 미구현(Phase 1)";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Analysis failed for {File}", video.FileName);
                    video.Status = "분석 오류";
                }

                index++;
                Progress = 0.5 + (0.5 * index / Math.Max(1, Videos.Count));
            }

            Progress = 1.0;
            StatusMessage = $"완료: {Videos.Count}개 영상 처리됨.";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "취소되었습니다.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scan/analyze failed");
            StatusMessage = $"오류: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
