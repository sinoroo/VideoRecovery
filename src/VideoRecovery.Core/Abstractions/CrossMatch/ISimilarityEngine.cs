namespace VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 시각적 유사도 계산 엔진. (요구사항 6 - 2단계 Visual Similarity)
/// Perceptual Hash / Histogram / SSIM 등을 종합한다.
/// </summary>
public interface ISimilarityEngine
{
    /// <summary>Perceptual hash 간 유사도 (0.0~1.0).</summary>
    double PerceptualHashSimilarity(ulong hashA, ulong hashB);

    /// <summary>히스토그램 상관도 (0.0~1.0).</summary>
    double HistogramSimilarity(IReadOnlyList<double> histogramA, IReadOnlyList<double> histogramB);

    /// <summary>SSIM 등 종합 시각 유사도 (0.0~1.0).</summary>
    Task<double> StructuralSimilarityAsync(string imagePathA, string imagePathB, CancellationToken cancellationToken = default);
}
