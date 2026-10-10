using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 단일 손상 소견. 하나의 영상에 여러 소견이 존재할 수 있다. (요구사항 7)
/// </summary>
public sealed class CorruptionFinding
{
    /// <summary>손상 유형.</summary>
    public CorruptionType Type { get; init; }

    /// <summary>손상 심각도.</summary>
    public HealthSeverity Severity { get; init; }

    /// <summary>영향을 받은 시작 프레임(알 수 없으면 null).</summary>
    public long? StartFrame { get; init; }

    /// <summary>영향을 받은 끝 프레임(알 수 없으면 null).</summary>
    public long? EndFrame { get; init; }

    /// <summary>이 소견에 대한 신뢰도 점수 (0.0 ~ 1.0). (요구사항 17-4)</summary>
    public double Confidence { get; init; }

    /// <summary>상세 설명.</summary>
    public string? Description { get; init; }
}
