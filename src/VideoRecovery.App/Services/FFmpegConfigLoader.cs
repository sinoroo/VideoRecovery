using System.Globalization;
using System.IO;
using VideoRecovery.Infrastructure.FFmpeg;

namespace VideoRecovery.App.Services;

/// <summary>ffmpeg_config.ini를 읽어 <see cref="FFmpegOptions"/>를 구성한다.</summary>
public static class FFmpegConfigLoader
{
    /// <summary>설정 파일을 읽는다. 없으면 기본값(PATH의 ffmpeg/ffprobe)을 사용한다.</summary>
    public static FFmpegOptions Load(string path)
    {
        var options = new FFmpegOptions();
        if (!File.Exists(path))
        {
            return options;
        }

        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#') || line.StartsWith('['))
            {
                continue;
            }

            var sep = line.IndexOf('=');
            if (sep <= 0)
            {
                continue;
            }

            var key = line[..sep].Trim();
            var value = line[(sep + 1)..].Trim();

            switch (key.ToLowerInvariant())
            {
                case "ffmpegpath":
                    if (value.Length > 0) options.FFmpegPath = value;
                    break;
                case "ffprobepath":
                    if (value.Length > 0) options.FFprobePath = value;
                    break;
                case "timeoutminutes":
                    if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var min) && min > 0)
                    {
                        options.Timeout = TimeSpan.FromMinutes(min);
                    }
                    break;
            }
        }

        return options;
    }
}
