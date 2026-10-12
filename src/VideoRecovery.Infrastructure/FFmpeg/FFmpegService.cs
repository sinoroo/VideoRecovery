using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;

namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>
/// ffmpeg CLI 기반 <see cref="IFFmpegService"/> 구현. 전체 디코드 테스트,
/// 프레임 추출, 컨테이너 재먹싱을 수행한다. 원본은 절대 수정하지 않는다.
/// </summary>
public sealed partial class FFmpegService : IFFmpegService
{
    private readonly FFmpegOptions _options;
    private readonly ILogger<FFmpegService> _logger;

    public FFmpegService(FFmpegOptions options, ILogger<FFmpegService> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DecodeError>> DecodeTestAsync(
        string path, CancellationToken cancellationToken = default)
    {
        // 전체 영상을 디코딩만 하고 출력은 버린다(-f null -). 오류는 stderr에 기록된다.
        var args = $"-v error -hide_banner -xerror -i \"{path}\" -f null -";

        var result = await ProcessRunner
            .RunAsync(_options.FFmpegPath, args, _options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Started)
        {
            _logger.LogWarning("ffmpeg not available: {Error}", result.StandardError);
            return new[]
            {
                new DecodeError { Message = "ffmpeg를 찾을 수 없습니다. FFmpeg 설치 후 프로그램을 재시작하세요." }
            };
        }

        return ParseDecodeErrors(result.StandardError);
    }

    public async Task<byte[]> ExtractFrameAsync(
        string path, long frameIndex, CancellationToken cancellationToken = default)
    {
        var temp = Path.Combine(Path.GetTempPath(), $"vr_frame_{Guid.NewGuid():N}.png");
        try
        {
            // 지정 프레임만 선택하여 PNG로 추출.
            var args =
                $"-v error -hide_banner -i \"{path}\" " +
                $"-vf \"select=eq(n\\,{frameIndex})\" -frames:v 1 -y \"{temp}\"";

            var result = await ProcessRunner
                .RunAsync(_options.FFmpegPath, args, _options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            if (result.Started && result.ExitCode == 0 && File.Exists(temp))
            {
                return await File.ReadAllBytesAsync(temp, cancellationToken).ConfigureAwait(false);
            }

            return Array.Empty<byte>();
        }
        finally
        {
            TryDelete(temp);
        }
    }

    public async Task<bool> RemuxAsync(
        string inputPath, string outputPath, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        // 스트림 복사로 컨테이너만 재구성(재인코딩 없음) → 인덱스/타임스탬프 복구.
        var args =
            $"-v error -hide_banner -err_detect ignore_err -i \"{inputPath}\" " +
            $"-c copy -movflags +faststart -y \"{outputPath}\"";

        var result = await ProcessRunner
            .RunAsync(_options.FFmpegPath, args, _options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return result.Started && result.ExitCode == 0 && File.Exists(outputPath);
    }

    public async Task<bool> ReencodeDropCorruptAsync(
        string inputPath, string outputPath, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        // 손상 패킷을 버리고(+discardcorrupt) 정상 프레임만 재디코딩 → 재인코딩.
        // genpts로 타임스탬프를 재생성하여 제거된 프레임 구간의 간격을 메운다.
        var args =
            $"-v error -hide_banner -err_detect ignore_err " +
            $"-fflags +discardcorrupt+genpts -i \"{inputPath}\" " +
            $"-map 0:v:0? -map 0:a:0? " +
            $"-c:v libx264 -preset veryfast -crf 20 " +
            $"-c:a aac -b:a 160k " +
            $"-fps_mode cfr -movflags +faststart -y \"{outputPath}\"";

        var result = await ProcessRunner
            .RunAsync(_options.FFmpegPath, args, _options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return result.Started && result.ExitCode == 0 && File.Exists(outputPath);
    }

    /// <summary>ffmpeg stderr에서 디코드 오류 라인을 추출한다.</summary>
    private static IReadOnlyList<DecodeError> ParseDecodeErrors(string stderr)
    {
        var errors = new List<DecodeError>();
        if (string.IsNullOrWhiteSpace(stderr))
        {
            return errors;
        }

        foreach (var raw in stderr.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var line = raw.Trim();
            if (line.Length == 0 || !LooksLikeError(line))
            {
                continue;
            }

            double? time = null;
            var timeMatch = TimeRegex().Match(line);
            if (timeMatch.Success
                && double.TryParse(timeMatch.Groups["t"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var t))
            {
                time = t;
            }

            errors.Add(new DecodeError { TimeSeconds = time, Message = line });
        }

        return errors;
    }

    private static bool LooksLikeError(string line)
        => line.Contains("error", StringComparison.OrdinalIgnoreCase)
           || line.Contains("corrupt", StringComparison.OrdinalIgnoreCase)
           || line.Contains("invalid", StringComparison.OrdinalIgnoreCase)
           || line.Contains("missing", StringComparison.OrdinalIgnoreCase)
           || line.Contains("concealing", StringComparison.OrdinalIgnoreCase);

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // 무시 — 임시 파일 정리.
        }
    }

    [GeneratedRegex(@"t:\s*(?<t>[0-9]+(\.[0-9]+)?)", RegexOptions.IgnoreCase)]
    private static partial Regex TimeRegex();
}
