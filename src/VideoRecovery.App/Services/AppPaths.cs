using System.IO;

namespace VideoRecovery.App.Services;

/// <summary>앱이 사용하는 파일/폴더 경로 모음.</summary>
public static class AppPaths
{
    /// <summary>실행 파일 기준 디렉터리.</summary>
    public static string BaseDirectory => AppContext.BaseDirectory;

    /// <summary>FFmpeg 설정 파일 경로.</summary>
    public static string FFmpegConfigPath => Path.Combine(BaseDirectory, "config", "ffmpeg_config.ini");

    /// <summary>로그 디렉터리.</summary>
    public static string LogDirectory => Path.Combine(BaseDirectory, "logs");

    /// <summary>로그 파일 경로(일자별 롤링).</summary>
    public static string LogFilePath => Path.Combine(LogDirectory, "videorecovery-.log");
}
