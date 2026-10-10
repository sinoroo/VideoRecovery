using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 종합 건강 분석 오케스트레이션. 비디오/프레임/오디오 분석을 결합한다. (요구사항 4, 16)
/// TODO(Phase 1): 하위 분석기 결과 병합 및 심각도 집계.
/// </summary>
public sealed class HealthAnalyzer : IHealthAnalyzer
{
    private readonly IVideoAnalyzer _videoAnalyzer;
    private readonly IFrameAnalyzer _frameAnalyzer;
    private readonly IAudioAnalyzer _audioAnalyzer;
    private readonly ILogger<HealthAnalyzer> _logger;

    public HealthAnalyzer(
        IVideoAnalyzer videoAnalyzer,
        IFrameAnalyzer frameAnalyzer,
        IAudioAnalyzer audioAnalyzer,
        ILogger<HealthAnalyzer> logger)
    {
        _videoAnalyzer = videoAnalyzer;
        _frameAnalyzer = frameAnalyzer;
        _audioAnalyzer = audioAnalyzer;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<HealthReport> RunAsync(
        VideoFileInfo file,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Health analysis requested for {FilePath}", file.FilePath);
        throw new NotImplementedException(
            "HealthAnalyzer.RunAsync: 비디오/프레임/오디오 분석 결합을 Phase 1 에서 구현한다.");
    }
}
