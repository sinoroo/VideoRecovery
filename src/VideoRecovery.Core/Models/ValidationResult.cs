namespace VideoRecovery.Core.Models;

/// <summary>
/// 복구 결과 재분석(검증) 결과. 복구 전/후 분석 결과를 비교할 수 있게 한다.
/// </summary>
public sealed class ValidationResult
{
    /// <summary>검증 대상(복구 결과물) 경로.</summary>
    public required string RecoveredPath { get; init; }

    /// <summary>복구 전 건강 리포트.</summary>
    public HealthReport? Before { get; init; }

    /// <summary>복구 후 건강 리포트.</summary>
    public HealthReport? After { get; init; }

    /// <summary>정상 재생 가능 여부.</summary>
    public bool Playable { get; init; }

    /// <summary>전체 프레임 디코딩 성공 여부.</summary>
    public bool FullyDecodable { get; init; }

    /// <summary>복구가 개선으로 판정되었는지 여부.</summary>
    public bool Improved { get; init; }

    /// <summary>개선 점수(0.0 ~ 1.0, 복구 전 대비).</summary>
    public double ImprovementScore { get; init; }

    /// <summary>요약.</summary>
    public string Summary { get; init; } = string.Empty;
}
