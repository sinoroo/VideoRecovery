using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.CrossMatch;

namespace VideoRecovery.CrossMatch;

/// <summary>
/// 시각 유사도 계산 엔진(2단계). pHash/Histogram/SSIM. (요구사항 6)
/// pHash/Histogram 은 순수 계산이므로 기본 구현을 제공하고, SSIM 은 OpenCV 에 위임한다.
/// </summary>
public sealed class SimilarityEngine : ISimilarityEngine
{
    private readonly ILogger<SimilarityEngine> _logger;

    public SimilarityEngine(ILogger<SimilarityEngine> logger) => _logger = logger;

    /// <inheritdoc />
    public double PerceptualHashSimilarity(ulong hashA, ulong hashB)
    {
        // 해밍 거리를 0.0~1.0 유사도로 변환 (64비트 pHash 기준).
        var distance = System.Numerics.BitOperations.PopCount(hashA ^ hashB);
        return 1.0 - (distance / 64.0);
    }

    /// <inheritdoc />
    public double HistogramSimilarity(IReadOnlyList<double> histogramA, IReadOnlyList<double> histogramB)
    {
        if (histogramA.Count == 0 || histogramA.Count != histogramB.Count)
        {
            return 0.0;
        }

        // 코사인 유사도 기반 히스토그램 상관도.
        double dot = 0, normA = 0, normB = 0;
        for (var i = 0; i < histogramA.Count; i++)
        {
            dot += histogramA[i] * histogramB[i];
            normA += histogramA[i] * histogramA[i];
            normB += histogramB[i] * histogramB[i];
        }

        if (normA == 0 || normB == 0)
        {
            return 0.0;
        }

        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }

    /// <inheritdoc />
    public Task<double> StructuralSimilarityAsync(string imagePathA, string imagePathB, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("SSIM requested {A} vs {B}", imagePathA, imagePathB);
        throw new NotImplementedException(
            "SimilarityEngine.StructuralSimilarityAsync: SSIM(OpenCV) 계산을 Phase 3 에서 구현한다.");
    }
}
