namespace VideoRecovery.Core.Models;

/// <summary>
/// 손상 영상의 특정 구간과 다른 영상 구간 간의 유사도 매칭 결과. (요구사항 6)
/// </summary>
public sealed class CrossVideoMatch
{
    /// <summary>손상(소스) 영상 경로.</summary>
    public required string SourceFilePath { get; init; }

    /// <summary>후보(reference) 영상 경로.</summary>
    public required string ReferenceFilePath { get; init; }

    /// <summary>소스 영상에서 비교한 시작 프레임.</summary>
    public long SourceStartFrame { get; init; }

    /// <summary>소스 영상에서 비교한 끝 프레임.</summary>
    public long SourceEndFrame { get; init; }

    /// <summary>reference 영상에서 매칭된 시작 프레임.</summary>
    public long ReferenceStartFrame { get; init; }

    /// <summary>reference 영상에서 매칭된 끝 프레임.</summary>
    public long ReferenceEndFrame { get; init; }

    /// <summary>시각적 유사도 점수 (0.0 ~ 1.0). pHash/Histogram/SSIM 종합.</summary>
    public double VisualSimilarity { get; init; }

    /// <summary>시간축(연속 프레임 패턴) 유사도 점수 (0.0 ~ 1.0).</summary>
    public double TemporalSimilarity { get; init; }

    /// <summary>종합 매칭 신뢰도 (0.0 ~ 1.0).</summary>
    public double Confidence { get; init; }
}
