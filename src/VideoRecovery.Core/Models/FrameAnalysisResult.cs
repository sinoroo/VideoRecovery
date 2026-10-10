namespace VideoRecovery.Core.Models;

/// <summary>
/// 프레임별 이상 점수. 0에 가까우면 정상, 1에 가까우면 이상. (요구사항 5)
/// </summary>
public sealed class FrameAnalysisResult
{
    /// <summary>프레임 인덱스.</summary>
    public long FrameIndex { get; init; }

    /// <summary>표시 타임스탬프(PTS) 기반 재생 위치.</summary>
    public TimeSpan Timestamp { get; init; }

    /// <summary>이상 점수 (0.0 ~ 1.0).</summary>
    public double AnomalyScore { get; init; }

    /// <summary><see cref="AnomalyScore"/> 가 임계값을 초과하여 이상으로 판정됐는지 여부.</summary>
    public bool IsAnomaly { get; init; }

    /// <summary>키프레임 여부.</summary>
    public bool IsKeyFrame { get; init; }

    /// <summary>이상 판정 근거(예: scene cut, block artifact, repeat).</summary>
    public string? Reason { get; init; }
}
