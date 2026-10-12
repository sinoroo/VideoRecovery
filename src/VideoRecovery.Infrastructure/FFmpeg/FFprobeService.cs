using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>
/// ffprobe CLI를 호출하여 JSON 메타데이터를 파싱하는 <see cref="IFFprobeService"/> 구현.
/// 바이너리가 없거나 파일이 열리지 않으면 <see cref="VideoMetadata.ProbeFailed"/>로 보고한다.
/// </summary>
public sealed class FFprobeService : IFFprobeService
{
    private readonly FFmpegOptions _options;
    private readonly ILogger<FFprobeService> _logger;

    public FFprobeService(FFmpegOptions options, ILogger<FFprobeService> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<VideoMetadata> ProbeAsync(string path, CancellationToken cancellationToken = default)
    {
        var args =
            "-v error -hide_banner -print_format json " +
            "-show_format -show_streams " +
            $"\"{path}\"";

        ProcessResult result;
        try
        {
            result = await ProcessRunner
                .RunAsync(_options.FFprobePath, args, _options.Timeout, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Failed(path, "ffprobe timed out.");
        }

        if (!result.Started)
        {
            _logger.LogWarning("ffprobe not available: {Error}", result.StandardError);
            return Failed(path, "ffprobe를 찾을 수 없습니다. FFmpeg를 설치했다면 프로그램을 재시작하거나 " +
                                "config/ffmpeg_config.ini에 ffprobe.exe의 전체 경로를 지정하세요.");
        }

        if (result.ExitCode != 0 || string.IsNullOrWhiteSpace(result.StandardOutput))
        {
            return Failed(path, result.StandardError.Trim());
        }

        try
        {
            return Parse(path, result.StandardOutput);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "ffprobe JSON 파싱 실패: {Path}", path);
            return Failed(path, $"JSON 파싱 실패: {ex.Message}");
        }
    }

    private static VideoMetadata Failed(string path, string? error) => new()
    {
        FullPath = path,
        ProbeFailed = true,
        ProbeError = error
    };

    private static VideoMetadata Parse(string path, string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var streams = new List<StreamInfo>();
        if (root.TryGetProperty("streams", out var streamArray))
        {
            foreach (var s in streamArray.EnumerateArray())
            {
                streams.Add(ParseStream(s));
            }
        }

        TimeSpan duration = default;
        long bitRate = 0;
        string formatName = string.Empty;
        var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (root.TryGetProperty("format", out var format))
        {
            formatName = GetString(format, "format_name");
            duration = TimeSpan.FromSeconds(GetDouble(format, "duration"));
            bitRate = GetLong(format, "bit_rate");

            if (format.TryGetProperty("tags", out var tagEl) && tagEl.ValueKind == JsonValueKind.Object)
            {
                foreach (var t in tagEl.EnumerateObject())
                {
                    tags[t.Name] = t.Value.ToString();
                }
            }
        }

        var video = streams.FirstOrDefault(x => x.Kind == StreamKind.Video);
        long frameCount = video is not null && duration > TimeSpan.Zero && video.Fps > 0
            ? (long)Math.Round(duration.TotalSeconds * video.Fps)
            : 0;

        return new VideoMetadata
        {
            FullPath = path,
            FormatName = formatName,
            Duration = duration,
            BitRate = bitRate,
            FrameCount = frameCount,
            Streams = streams,
            Tags = tags
        };
    }

    private static StreamInfo ParseStream(JsonElement s)
    {
        var codecType = GetString(s, "codec_type");
        var kind = codecType switch
        {
            "video" => StreamKind.Video,
            "audio" => StreamKind.Audio,
            "subtitle" => StreamKind.Subtitle,
            "data" => StreamKind.Data,
            _ => StreamKind.Unknown
        };

        return new StreamInfo
        {
            Index = GetInt(s, "index"),
            Kind = kind,
            Codec = GetString(s, "codec_name"),
            Width = GetInt(s, "width"),
            Height = GetInt(s, "height"),
            Fps = ParseRational(GetString(s, "avg_frame_rate")),
            SampleRate = ParseInt(GetString(s, "sample_rate")),
            Channels = GetInt(s, "channels"),
            TimeBase = GetString(s, "time_base"),
            BitRate = GetLong(s, "bit_rate")
        };
    }

    private static string GetString(JsonElement el, string name)
        => el.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString() ?? string.Empty
            : string.Empty;

    private static int GetInt(JsonElement el, string name)
        => el.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number
            ? v.GetInt32()
            : 0;

    private static long GetLong(JsonElement el, string name)
    {
        if (!el.TryGetProperty(name, out var v))
        {
            return 0;
        }

        return v.ValueKind switch
        {
            JsonValueKind.Number => v.GetInt64(),
            JsonValueKind.String => ParseLong(v.GetString()),
            _ => 0
        };
    }

    private static double GetDouble(JsonElement el, string name)
    {
        if (!el.TryGetProperty(name, out var v))
        {
            return 0;
        }

        return v.ValueKind switch
        {
            JsonValueKind.Number => v.GetDouble(),
            JsonValueKind.String => double.TryParse(v.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0,
            _ => 0
        };
    }

    private static int ParseInt(string value)
        => int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : 0;

    private static long ParseLong(string? value)
        => long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var l) ? l : 0;

    /// <summary>"30000/1001" 같은 유리수 프레임레이트를 double로 변환.</summary>
    private static double ParseRational(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "0/0")
        {
            return 0;
        }

        var parts = value.Split('/');
        if (parts.Length == 2
            && double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var num)
            && double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var den)
            && den != 0)
        {
            return num / den;
        }

        return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var single) ? single : 0;
    }
}
