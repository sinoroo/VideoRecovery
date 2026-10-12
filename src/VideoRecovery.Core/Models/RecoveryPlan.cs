namespace VideoRecovery.Core.Models;

/// <summary>
/// Recovery Planner가 생성하는 복구 계획. 손상 유형별로 정렬된
/// 복구 단계와 전체 복구 가능성/추천 참조를 담는다.
/// </summary>
public sealed class RecoveryPlan
{
    /// <summary>대상 파일 경로.</summary>
    public required string FullPath { get; init; }

    /// <summary>정렬된 복구 단계(안전한 레벨부터).</summary>
    public IReadOnlyList<RecoveryStep> Steps { get; init; } = Array.Empty<RecoveryStep>();

    /// <summary>전체 복구 가능성(0.0 ~ 1.0).</summary>
    public double RecoveryProbability { get; init; }

    /// <summary>자동 추천 참조 후보(없으면 null).</summary>
    public ReferenceCandidate? RecommendedReference { get; init; }

    /// <summary>사용자 표시용 요약.</summary>
    public string Summary { get; init; } = string.Empty;
}
