using System.IO;

namespace VideoRecovery.App.Services;

/// <summary>설정/로그/작업 영역 경로를 해석한다.</summary>
public static class AppPaths
{
    public static string BaseDirectory => AppContext.BaseDirectory;

    public static string ConfigDirectory => Path.Combine(BaseDirectory, "config");

    public static string FFmpegConfigPath => Path.Combine(ConfigDirectory, "ffmpeg_config.ini");

    public static string LogDirectory => Path.Combine(BaseDirectory, "logs");

    public static string LogFilePath => Path.Combine(LogDirectory, "videorecovery-.log");

    /// <summary>복구 작업 영역(원본과 분리). (요구사항 2.1)</summary>
    public static string WorkspaceDirectory => Path.Combine(BaseDirectory, "workspace");
}
