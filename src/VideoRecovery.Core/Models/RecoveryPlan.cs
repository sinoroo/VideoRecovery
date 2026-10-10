using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 자동 복구 의사결정 결과. 사용자에게 추천 방법/예상 결과를 제시한다. (요구사항 9, 10)
/// </summary>
public sealed class RecoveryPlan
{
    /// <summary>복구 대상 파일 경로.</summary>
    public required string FilePath { get; init; }

    /// <summary>손상 정도.</summary>
    public HealthSeverity Severity { get; init; }

    /// <summary>복구 가능성 (0.0 ~ 1.0). 예: 0.87.</summary>
    public double RecoveryProbability { get; init; }

    /// <summary>자동 추천되는 1순위 복구 단계.</summary>
    public RecoveryLevel RecommendedLevel { get; init; }

    /// <summary>순차적으로 시도할 복구 단계 목록(실패 시 다음 단계). (요구사항 17-6)</summary>
    public IReadOnlyList<RecoveryStep> Steps { get; init; } = [];

    /// <summary>자동 복구가 불확실하여 사용자 승인이 필요한지 여부. (요구사항 17-5)</summary>
    public bool RequiresUserApproval { get; init; }

    /// <summary>예상 결과 요약(사용자 표시용).</summary>
    public string? ExpectedOutcome { get; init; }
}
