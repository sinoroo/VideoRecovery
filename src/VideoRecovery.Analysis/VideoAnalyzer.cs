using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 컨테이너/비디오 스트림 분석 + 디코드 테스트. Video Analysis Agent. (요구사항 4, 16)
/// TODO(Phase 1): 디코드 구간 수집 및 심각도 산정 로직 구현.
/// </summary>
public sealed class VideoAnalyzer : IVideoAnalyzer
{
    private readonly IFFmpegService _ffmpeg;
    private readonly ILogger<VideoAnalyzer> _logger;

    public VideoAnalyzer(IFFmpegService ffmpeg, ILogger<VideoAnalyzer> logger)
    {
        _ffmpeg = ffmpeg;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<HealthReport> AnalyzeAsync(
        VideoFileInfo file,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Analyze requested for {FilePath}", file.FilePath);
        throw new NotImplementedException(
            "VideoAnalyzer.AnalyzeAsync: 디코드 테스트 기반 HealthReport 생성을 Phase 1 에서 구현한다.");
    }
}
