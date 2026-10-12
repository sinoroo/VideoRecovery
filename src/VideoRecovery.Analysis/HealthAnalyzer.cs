using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 단일 영상의 건강 상태 종합 분석기. 메타데이터 → 디코드 테스트 → 프레임 분석 →
/// A/V 동기 순으로 수행하고 하나의 <see cref="HealthReport"/>로 합친다. 원본 미변경.
/// </summary>
public sealed class HealthAnalyzer : IHealthAnalyzer
{
    private readonly IVideoAnalyzer _videoAnalyzer;
    private readonly IFrameAnalyzer _frameAnalyzer;
    private readonly IAudioAnalyzer _audioAnalyzer;
    private readonly IFFmpegService _ffmpeg;
    private readonly ILogger<HealthAnalyzer> _logger;

    public HealthAnalyzer(
        IVideoAnalyzer videoAnalyzer,
        IFrameAnalyzer frameAnalyzer,
        IAudioAnalyzer audioAnalyzer,
        IFFmpegService ffmpeg,
        ILogger<HealthAnalyzer> logger)
    {
        _videoAnalyzer = videoAnalyzer;
        _frameAnalyzer = frameAnalyzer;
        _audioAnalyzer = audioAnalyzer;
        _ffmpeg = ffmpeg;
        _logger = logger;
    }

    public async Task<HealthReport> AnalyzeAsync(string path, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("건강 분석 시작: {Path}", path);

        var metadata = await _videoAnalyzer.AnalyzeAsync(path, cancellationToken).ConfigureAwait(false);

        // 컨테이너 자체를 열 수 없으면 디코드 테스트는 생략하고 즉시 심각으로 보고.
        if (metadata.ProbeFailed)
        {
            return new HealthReport
            {
                FullPath = path,
                Metadata = metadata,
                Severity = HealthSeverity.Severe,
                Summary = $"컨테이너를 열 수 없음: {metadata.ProbeError}"
            };
        }

        var fps = metadata.PrimaryVideo?.Fps ?? 0;

        var decodeErrors = await _ffmpeg.DecodeTestAsync(path, cancellationToken).ConfigureAwait(false);
        var segments = BuildSegments(decodeErrors, metadata.FrameCount, fps);

        var frameAnalysis = await _frameAnalyzer.AnalyzeAsync(path, cancellationToken).ConfigureAwait(false);
        var avSync = await _audioAnalyzer.MeasureAvSyncAsync(metadata, cancellationToken).ConfigureAwait(false);

        var severity = ComputeSeverity(segments, metadata.FrameCount);

        var report = new HealthReport
        {
            FullPath = path,
            Metadata = metadata,
            DecodeSegments = segments,
            FrameAnalysis = frameAnalysis,
            AvSyncOffsetSeconds = avSync,
            Severity = severity,
            Summary = BuildSummary(metadata, segments, severity)
        };

        _logger.LogInformation(
            "건강 분석 완료: {Path} — 심각도 {Severity}, 오류구간 {Errors}",
            path, severity, report.ErrorSegmentCount);

        return report;
    }

    /// <summary>디코드 오류 목록을 정상/오류 연속 구간으로 변환한다.</summary>
    private static IReadOnlyList<DecodeSegment> BuildSegments(
        IReadOnlyList<DecodeError> errors, long frameCount, double fps)
    {
        if (errors.Count == 0)
        {
            return frameCount > 0
                ? new[] { new DecodeSegment { StartFrame = 0, EndFrame = frameCount - 1, IsError = false } }
                : Array.Empty<DecodeSegment>();
        }

        // 오류 시각을 프레임 인덱스로 환산하여 정렬.
        var errorFrames = errors
            .Select(e => e.FrameIndex ?? (e.TimeSeconds is { } t && fps > 0 ? (long)Math.Round(t * fps) : -1))
            .Where(f => f >= 0)
            .Distinct()
            .OrderBy(f => f)
            .ToList();

        if (errorFrames.Count == 0)
        {
            // 프레임 환산이 불가능하면 오류 1건만 기록.
            return new[]
            {
                new DecodeSegment { StartFrame = 0, EndFrame = 0, IsError = true, Detail = errors[0].Message }
            };
        }

        var segments = new List<DecodeSegment>();
        long cursor = 0;
        var i = 0;

        while (i < errorFrames.Count)
        {
            var start = errorFrames[i];

            // 정상 구간: cursor ~ start-1.
            if (start > cursor)
            {
                segments.Add(new DecodeSegment { StartFrame = cursor, EndFrame = start - 1, IsError = false });
            }

            // 연속 오류 프레임을 하나의 구간으로 묶는다.
            var end = start;
            while (i + 1 < errorFrames.Count && errorFrames[i + 1] == end + 1)
            {
                end = errorFrames[++i];
            }

            segments.Add(new DecodeSegment { StartFrame = start, EndFrame = end, IsError = true, Detail = "Decode Error" });
            cursor = end + 1;
            i++;
        }

        // 마지막 정상 구간.
        if (frameCount > 0 && cursor < frameCount)
        {
            segments.Add(new DecodeSegment { StartFrame = cursor, EndFrame = frameCount - 1, IsError = false });
        }

        return segments;
    }

    private static HealthSeverity ComputeSeverity(IReadOnlyList<DecodeSegment> segments, long frameCount)
    {
        var errorFrames = segments.Where(s => s.IsError).Sum(s => s.FrameLength);
        if (errorFrames == 0)
        {
            return HealthSeverity.Healthy;
        }

        if (frameCount <= 0)
        {
            return HealthSeverity.Moderate;
        }

        var ratio = (double)errorFrames / frameCount;
        return ratio switch
        {
            > 0.2 => HealthSeverity.Severe,
            > 0.05 => HealthSeverity.Moderate,
            _ => HealthSeverity.Minor
        };
    }

    private static string BuildSummary(
        VideoMetadata metadata, IReadOnlyList<DecodeSegment> segments, HealthSeverity severity)
    {
        var errorCount = segments.Count(s => s.IsError);
        var video = metadata.PrimaryVideo;
        var res = video is not null ? $"{video.Width}x{video.Height}@{video.Fps:0.##}fps" : "N/A";
        return $"{res}, {metadata.Duration:hh\\:mm\\:ss}, 오류구간 {errorCount}개, 심각도 {severity}";
    }
}
