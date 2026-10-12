namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>FFmpeg/FFprobe 실행 파일 경로 설정.</summary>
public sealed class FFmpegOptions
{
    /// <summary>ffmpeg 실행 파일 경로(기본: PATH의 "ffmpeg").</summary>
    public string FFmpegPath { get; set; } = "ffmpeg";

    /// <summary>ffprobe 실행 파일 경로(기본: PATH의 "ffprobe").</summary>
    public string FFprobePath { get; set; } = "ffprobe";

    /// <summary>외부 프로세스 타임아웃.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);
}
