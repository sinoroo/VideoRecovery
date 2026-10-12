using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// 복구 계획 수립기. 손상 유형에 따라 복구 전략을 자동 결정한다.
/// Recovery Planning Agent의 인터페이스.
/// </summary>
public interface IRecoveryPlanner
{
    /// <summary>손상 분류와 참조 후보로부터 복구 계획을 생성한다.</summary>
    RecoveryPlan CreatePlan(
        CorruptionReport corruption,
        IReadOnlyList<ReferenceCandidate> candidates);
}
