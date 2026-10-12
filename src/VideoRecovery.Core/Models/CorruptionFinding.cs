using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>손상 분류기가 생성하는 개별 손상 소견.</summary>
public sealed class CorruptionFinding
{
    /// <summary>손상 유형(단일).</summary>
    public CorruptionType Type { get; init; }

    /// <summary>영향 받은 시작 프레임(알 수 없으면 null).</summary>
    public long? StartFrame { get; init; }

    /// <summary>영향 받은 끝 프레임(알 수 없으면 null).</summary>
    public long? EndFrame { get; init; }

    /// <summary>분류 신뢰도(0.0 ~ 1.0).</summary>
    public double Confidence { get; init; }

    /// <summary>근거 설명.</summary>
    public string Detail { get; init; } = string.Empty;
}
