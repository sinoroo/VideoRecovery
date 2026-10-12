using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>복구 계획을 구성하는 단일 복구 단계.</summary>
public sealed class RecoveryStep
{
    /// <summary>복구 레벨.</summary>
    public RecoveryLevel Level { get; init; }

    /// <summary>이 단계가 대응하는 손상 유형.</summary>
    public CorruptionType TargetType { get; init; }

    /// <summary>사람이 읽을 수 있는 설명.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>참조 영상 경로(Cross-video 단계에서 사용).</summary>
    public string? ReferencePath { get; init; }

    /// <summary>예상 성공 신뢰도(0.0 ~ 1.0).</summary>
    public double EstimatedConfidence { get; init; }
}
