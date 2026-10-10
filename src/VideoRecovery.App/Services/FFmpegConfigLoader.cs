using System.IO;
using VideoRecovery.Infrastructure.FFmpeg;

namespace VideoRecovery.App.Services;

/// <summary>
/// 간단한 key=value INI 로더. ffmpeg_config.ini 에서 실행 파일 경로를 읽는다.
/// 설정은 하드코딩하지 않는다. (요구사항 2.2)
/// </summary>
public static class FFmpegConfigLoader
{
    public static FFmpegOptions Load(string iniPath)
    {
        var options = new FFmpegOptions();
        if (!File.Exists(iniPath))
        {
            return options;
        }

        foreach (var raw in File.ReadLines(iniPath))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#') || line.StartsWith(';'))
            {
                continue;
            }

            var idx = line.IndexOf('=');
            if (idx <= 0)
            {
                continue;
            }

            var key = line[..idx].Trim();
            var value = line[(idx + 1)..].Trim();

            switch (key.ToLowerInvariant())
            {
                case "ffmpegpath":
                    options.FFmpegPath = value;
                    break;
                case "ffprobepath":
                    options.FFprobePath = value;
                    break;
            }
        }

        return options;
    }
}
