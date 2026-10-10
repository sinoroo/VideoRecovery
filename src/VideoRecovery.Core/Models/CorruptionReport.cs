using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 손상 유형 분류 결과 집합. (요구사항 7)
/// </summary>
public sealed class CorruptionReport
{
    /// <summary>분석 대상 파일 경로.</summary>
    public required string FilePath { get; init; }

    /// <summary>개별 손상 소견 목록.</summary>
    public IReadOnlyList<CorruptionFinding> Findings { get; init; } = [];

    /// <summary>모든 소견을 합집합한 손상 유형.</summary>
    public CorruptionType Aggregate =>
        Findings.Aggregate(CorruptionType.None, (acc, f) => acc | f.Type);

    /// <summary>최고 심각도.</summary>
    public HealthSeverity MaxSeverity =>
        Findings.Count == 0 ? HealthSeverity.Healthy : Findings.Max(f => f.Severity);
}
