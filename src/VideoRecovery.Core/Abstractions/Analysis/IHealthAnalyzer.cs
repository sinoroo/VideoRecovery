using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>
/// 영상 건강 상태 종합 분석기. 메타데이터/디코드 테스트/프레임 분석을
/// 조합하여 하나의 <see cref="HealthReport"/>를 만든다.
/// </summary>
public interface IHealthAnalyzer
{
    /// <summary>단일 영상의 건강 상태를 종합 분석한다(원본 미변경).</summary>
    Task<HealthReport> AnalyzeAsync(string path, CancellationToken cancellationToken = default);
}
