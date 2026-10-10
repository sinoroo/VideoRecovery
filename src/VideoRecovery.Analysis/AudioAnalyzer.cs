using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 오디오 스트림 분석 및 A/V 동기 측정. (요구사항 4)
/// TODO(Phase 1): 오디오 PTS 와 비디오 PTS 비교로 동기 오차 산출.
/// </summary>
public sealed class AudioAnalyzer : IAudioAnalyzer
{
    private readonly ILogger<AudioAnalyzer> _logger;

    public AudioAnalyzer(ILogger<AudioAnalyzer> logger) => _logger = logger;

    /// <inheritdoc />
    public Task<double> MeasureAvSyncAsync(VideoFileInfo file, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("A/V sync measurement requested for {FilePath}", file.FilePath);
        throw new NotImplementedException(
            "AudioAnalyzer.MeasureAvSyncAsync: A/V 동기 측정을 Phase 1 에서 구현한다.");
    }
}
