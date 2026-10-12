namespace VideoRecovery.Core.Models;

/// <summary>
/// 복구 참조로 사용할 수 있는 정상 영상 후보. Confidence Score가 가장 높은
/// 후보를 Recovery Planner가 자동 추천한다.
/// </summary>
public sealed class ReferenceCandidate
{
    /// <summary>참조 영상 경로.</summary>
    public required string ReferencePath { get; init; }

    /// <summary>메타데이터 유사도(코덱/해상도/FPS 등, 0.0 ~ 1.0).</summary>
    public double MetadataSimilarity { get; init; }

    /// <summary>시각 유사도(0.0 ~ 1.0).</summary>
    public double VisualSimilarity { get; init; }

    /// <summary>종합 신뢰도(0.0 ~ 1.0).</summary>
    public double Confidence { get; init; }

    /// <summary>추천 사유.</summary>
    public string Reason { get; init; } = string.Empty;
}
