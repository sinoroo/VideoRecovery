using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>
/// 종합 건강 분석 오케스트레이션. Health Analysis 전체 흐름을 조율한다. (요구사항 4, 16)
/// </summary>
public interface IHealthAnalyzer
{
    /// <summary>
    /// 비디오/프레임/오디오 분석을 결합하여 최종 건강 리포트를 생성한다.
    /// </summary>
    Task<HealthReport> RunAsync(
        VideoFileInfo file,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);
}
