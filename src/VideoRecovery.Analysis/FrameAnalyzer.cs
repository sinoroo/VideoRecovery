using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 프레임 이상 탐지기. Phase 1에서는 ffmpeg 디코드 오류의 시각을 프레임 인덱스로
/// 환산하여 Analysis Score를 산출한다. 시각적(픽셀 단위) 이상 탐지는 OpenCV 연동
/// (Phase 3)에서 확장한다.
/// </summary>
public sealed class FrameAnalyzer : IFrameAnalyzer
{
    private const double AnomalyScore = 0.9;
    private const double HealthyScore = 0.05;

    private readonly IFFmpegService _ffmpeg;
    private readonly IFFprobeService _ffprobe;
    private readonly ILogger<FrameAnalyzer> _logger;

    public FrameAnalyzer(IFFmpegService ffmpeg, IFFprobeService ffprobe, ILogger<FrameAnalyzer> logger)
    {
        _ffmpeg = ffmpeg;
        _ffprobe = ffprobe;
        _logger = logger;
    }

    public async Task<FrameAnalysisResult> AnalyzeAsync(string path, CancellationToken cancellationToken = default)
    {
        var metadata = await _ffprobe.ProbeAsync(path, cancellationToken).ConfigureAwait(false);
        var fps = metadata.PrimaryVideo?.Fps ?? 0;

        var errors = await _ffmpeg.DecodeTestAsync(path, cancellationToken).ConfigureAwait(false);

        var frames = new List<FrameAnomaly>();
        foreach (var error in errors)
        {
            long frameIndex = error.FrameIndex
                ?? (error.TimeSeconds is { } t && fps > 0 ? (long)Math.Round(t * fps) : -1);

            frames.Add(new FrameAnomaly
            {
                FrameIndex = frameIndex,
                PtsSeconds = error.TimeSeconds ?? 0,
                AnomalyScore = AnomalyScore,
                IsAnomaly = true,
                Reason = error.Message
            });
        }

        _logger.LogInformation("프레임 이상 탐지: {Path} — 이상 {Count}건", path, frames.Count);

        return new FrameAnalysisResult
        {
            FullPath = path,
            Threshold = 0.5,
            Frames = frames
        };
    }

    /// <summary>테스트/시각 분석 단계에서 정상 프레임 점수를 생성할 때 사용하는 기본값.</summary>
    public static double DefaultHealthyScore => HealthyScore;
}
