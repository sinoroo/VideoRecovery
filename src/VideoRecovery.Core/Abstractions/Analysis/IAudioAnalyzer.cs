using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>오디오 스트림 분석기(샘플레이트/채널/동기화).</summary>
public interface IAudioAnalyzer
{
    /// <summary>오디오/비디오 동기화 오차(초)를 측정한다. null이면 측정 불가.</summary>
    Task<double?> MeasureAvSyncAsync(
        VideoMetadata metadata, CancellationToken cancellationToken = default);
}
