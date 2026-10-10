using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 복구 계획의 단일 단계. Recovery Planner 가 손상 유형별로 생성한다. (요구사항 9)
/// </summary>
public sealed class RecoveryStep
{
    /// <summary>적용할 복구 단계(Level).</summary>
    public RecoveryLevel Level { get; init; }

    /// <summary>이 단계가 대상으로 하는 손상 유형.</summary>
    public CorruptionType Target { get; init; }

    /// <summary>Cross-Video/AI 복구 시 사용할 reference(있다면).</summary>
    public ReferenceCandidate? Reference { get; init; }

    /// <summary>단계 설명(사용자 표시용).</summary>
    public string? Description { get; init; }
}
