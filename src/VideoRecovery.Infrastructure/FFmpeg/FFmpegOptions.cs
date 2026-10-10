namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>
/// ffmpeg/ffprobe 실행 파일 경로 설정. INI/환경변수에서 로드하며 하드코딩하지 않는다.
/// (요구사항 2.2, 설정 외부화)
/// </summary>
public sealed class FFmpegOptions
{
    /// <summary>ffmpeg 실행 파일 경로(기본값은 PATH 의 "ffmpeg").</summary>
    public string FFmpegPath { get; set; } = "ffmpeg";

    /// <summary>ffprobe 실행 파일 경로(기본값은 PATH 의 "ffprobe").</summary>
    public string FFprobePath { get; set; } = "ffprobe";
}
