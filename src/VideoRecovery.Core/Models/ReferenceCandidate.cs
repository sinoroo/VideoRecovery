namespace VideoRecovery.Core.Models;

/// <summary>
/// 복구 reference 후보. Confidence Score 기준으로 자동 추천된다. (요구사항 6, 8 Level 3)
/// </summary>
public sealed class ReferenceCandidate
{
    /// <summary>reference 로 사용할 영상 경로.</summary>
    public required string FilePath { get; init; }

    /// <summary>복구 신뢰도 점수 (0.0 ~ 1.0).</summary>
    public double Confidence { get; init; }

    /// <summary>후보 선정 근거(메타데이터/시각/시간축 매칭 요약).</summary>
    public string? Rationale { get; init; }

    /// <summary>관련 Cross-Video 매칭 상세(있다면).</summary>
    public IReadOnlyList<CrossVideoMatch> Matches { get; init; } = [];
}
