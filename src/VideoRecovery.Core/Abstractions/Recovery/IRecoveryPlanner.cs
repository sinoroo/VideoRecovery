using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// 복구 전략 수립. 손상 유형에 따라 복구 단계 순서를 결정한다.
/// Recovery Planning Agent 의 계약. (요구사항 9, 16)
/// </summary>
public interface IRecoveryPlanner
{
    /// <summary>
    /// 손상 리포트와 reference 후보를 바탕으로 복구 계획을 생성한다.
    /// 복구 가능성/추천 방법/예상 결과를 포함한다. (요구사항 9, 10)
    /// </summary>
    Task<RecoveryPlan> CreatePlanAsync(
        VideoFileInfo file,
        CorruptionReport corruption,
        IReadOnlyList<ReferenceCandidate> references,
        CancellationToken cancellationToken = default);
}
