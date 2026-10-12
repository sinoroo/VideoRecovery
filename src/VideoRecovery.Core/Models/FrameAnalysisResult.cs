namespace VideoRecovery.Core.Models;

/// <summary>
/// 단일 프레임의 이상 탐지 결과. <see cref="AnomalyScore"/>는 0(정상)~1(이상)
/// 범위이며, <see cref="FrameAnalysisResult.Threshold"/>를 초과하면 이상으로 간주한다.
/// </summary>
public sealed class FrameAnomaly
{
    /// <summary>프레임 인덱스(0-based).</summary>
    public long FrameIndex { get; init; }

    /// <summary>프레젠테이션 타임스탬프(초).</summary>
    public double PtsSeconds { get; init; }

    /// <summary>이상 점수(0.0 ~ 1.0).</summary>
    public double AnomalyScore { get; init; }

    /// <summary>이상으로 판정되었는지 여부.</summary>
    public bool IsAnomaly { get; init; }

    /// <summary>탐지된 현상 설명(예: 급격한 화면 변화, 중복 프레임).</summary>
    public string? Reason { get; init; }
}

/// <summary>프레임 이상 탐지 전체 결과.</summary>
public sealed class FrameAnalysisResult
{
    /// <summary>대상 파일 경로.</summary>
    public required string FullPath { get; init; }

    /// <summary>이상 판정 임계값(이 값 초과 시 이상).</summary>
    public double Threshold { get; init; } = 0.5;

    /// <summary>프레임별 점수.</summary>
    public IReadOnlyList<FrameAnomaly> Frames { get; init; } = Array.Empty<FrameAnomaly>();

    /// <summary>이상으로 판정된 프레임만 반환.</summary>
    public IEnumerable<FrameAnomaly> Anomalies => Frames.Where(f => f.IsAnomaly);
}
