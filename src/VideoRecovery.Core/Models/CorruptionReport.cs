using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 단일 영상의 손상 분류 종합 결과. 여러 유형이 동시에 존재할 수 있으므로
/// <see cref="Types"/>는 플래그 조합이다.
/// </summary>
public sealed class CorruptionReport
{
    /// <summary>대상 파일 경로.</summary>
    public required string FullPath { get; init; }

    /// <summary>탐지된 손상 유형의 조합.</summary>
    public CorruptionType Types { get; init; } = CorruptionType.None;

    /// <summary>개별 소견 목록.</summary>
    public IReadOnlyList<CorruptionFinding> Findings { get; init; }
        = Array.Empty<CorruptionFinding>();

    /// <summary>종합 심각도.</summary>
    public HealthSeverity Severity { get; init; } = HealthSeverity.Healthy;

    /// <summary>손상 없음 여부.</summary>
    public bool IsHealthy => Types == CorruptionType.None;
}
