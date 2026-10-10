namespace VideoRecovery.Core.Models;

/// <summary>
/// 복구 결과 검증 리포트. 복구 전/후 분석 결과를 비교한다. (요구사항 11, 17-10)
/// </summary>
public sealed class ValidationResult
{
    /// <summary>검증 대상(복구된) 파일 경로.</summary>
    public required string FilePath { get; init; }

    /// <summary>정상 재생 가능 여부.</summary>
    public bool PlaysBackCleanly { get; init; }

    /// <summary>전체 프레임 디코딩 성공 여부.</summary>
    public bool AllFramesDecoded { get; init; }

    /// <summary>복구 후 프레임 수.</summary>
    public long FrameCount { get; init; }

    /// <summary>복구 후 재생 길이.</summary>
    public TimeSpan Duration { get; init; }

    /// <summary>오디오/비디오 동기 오차(초).</summary>
    public double AvSyncOffsetSeconds { get; init; }

    /// <summary>복구 전 health 리포트(비교용).</summary>
    public HealthReport? Before { get; init; }

    /// <summary>복구 후 health 리포트(비교용).</summary>
    public HealthReport? After { get; init; }

    /// <summary>손상 구간 개선 비율 (0.0 ~ 1.0).</summary>
    public double ImprovementRatio { get; init; }

    /// <summary>종합 검증 통과 여부.</summary>
    public bool Passed { get; init; }
}
