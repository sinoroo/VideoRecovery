using CommunityToolkit.Mvvm.ComponentModel;
using VideoRecovery.Core.Models;

namespace VideoRecovery.App.ViewModels;

/// <summary>
/// 스캔된 영상 한 건을 표시하기 위한 행 ViewModel. (요구사항 3.1, 4)
/// </summary>
public partial class ScannedVideoViewModel : ObservableObject
{
    public ScannedVideoViewModel(VideoFileInfo info)
    {
        Info = info;
    }

    /// <summary>원본 스캔 정보.</summary>
    public VideoFileInfo Info { get; }

    /// <summary>파일명.</summary>
    public string FileName => Info.FileName;

    /// <summary>파일 크기(MB).</summary>
    public double SizeMb => Info.SizeBytes / (1024.0 * 1024.0);

    /// <summary>코덱 요약(메타데이터가 있을 때).</summary>
    public string Codec => Info.Metadata?.VideoCodec ?? "-";

    /// <summary>해상도 요약.</summary>
    public string Resolution =>
        Info.Metadata is { Width: > 0, Height: > 0 } m ? $"{m.Width}x{m.Height}" : "-";

    /// <summary>분석 상태 메시지(진행/완료/오류).</summary>
    [ObservableProperty]
    private string _status = "대기";

    /// <summary>건강 분석 결과(완료 시).</summary>
    [ObservableProperty]
    private HealthReport? _health;
}
