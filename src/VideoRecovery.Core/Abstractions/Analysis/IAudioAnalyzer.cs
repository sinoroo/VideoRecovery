using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>
/// 오디오 스트림 분석 및 A/V 동기 검사. (요구사항 4 - Audio Stream)
/// </summary>
public interface IAudioAnalyzer
{
    /// <summary>
    /// 오디오 스트림을 분석하고 오디오/비디오 동기 오차(초)를 반환한다.
    /// </summary>
    Task<double> MeasureAvSyncAsync(VideoFileInfo file, CancellationToken cancellationToken = default);
}
