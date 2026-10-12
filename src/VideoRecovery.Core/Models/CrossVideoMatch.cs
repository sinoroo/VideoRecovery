namespace VideoRecovery.Core.Models;

/// <summary>
/// 손상 영상의 특정 구간이 다른 영상의 구간과 높은 유사도를 보이는 매치.
/// Cross-video contamination(다른 영상 데이터 혼입) 판단 근거가 된다.
/// </summary>
public sealed class CrossVideoMatch
{
    /// <summary>손상(쿼리) 영상 경로.</summary>
    public required string SourcePath { get; init; }

    /// <summary>후보(참조) 영상 경로.</summary>
    public required string ReferencePath { get; init; }

    /// <summary>손상 영상에서의 시작 프레임.</summary>
    public long SourceStartFrame { get; init; }

    /// <summary>참조 영상에서의 시작 프레임.</summary>
    public long ReferenceStartFrame { get; init; }

    /// <summary>비교한 연속 프레임 길이.</summary>
    public int Length { get; init; }

    /// <summary>시각 유사도(0.0 ~ 1.0).</summary>
    public double VisualSimilarity { get; init; }

    /// <summary>시간축(연속 패턴) 유사도(0.0 ~ 1.0).</summary>
    public double TemporalSimilarity { get; init; }

    /// <summary>종합 신뢰도(0.0 ~ 1.0).</summary>
    public double Confidence { get; init; }
}
