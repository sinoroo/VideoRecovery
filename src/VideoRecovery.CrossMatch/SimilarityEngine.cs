using VideoRecovery.Core.Abstractions.CrossMatch;
using VideoRecovery.Core.Models;

namespace VideoRecovery.CrossMatch;

/// <summary>
/// 유사도 엔진. 메타데이터 특성 유사도와 특징 벡터(히스토그램 등) 유사도를 산출한다.
/// </summary>
public sealed class SimilarityEngine : ISimilarityEngine
{
    public double MetadataSimilarity(VideoMetadata a, VideoMetadata b)
    {
        if (a.ProbeFailed || b.ProbeFailed)
        {
            return 0;
        }

        double score = 0;
        double weight = 0;

        var va = a.PrimaryVideo;
        var vb = b.PrimaryVideo;

        if (va is not null && vb is not null)
        {
            // 코덱 일치(가중 0.35).
            score += Weighted(string.Equals(va.Codec, vb.Codec, StringComparison.OrdinalIgnoreCase) ? 1 : 0, 0.35, ref weight);

            // 해상도 일치(가중 0.30).
            var resMatch = va.Width == vb.Width && va.Height == vb.Height ? 1 : 0;
            score += Weighted(resMatch, 0.30, ref weight);

            // FPS 근접(가중 0.20).
            score += Weighted(Closeness(va.Fps, vb.Fps, 1.0), 0.20, ref weight);
        }

        // 포맷 이름 일치(가중 0.15).
        score += Weighted(string.Equals(a.FormatName, b.FormatName, StringComparison.OrdinalIgnoreCase) ? 1 : 0, 0.15, ref weight);

        return weight > 0 ? score / weight : 0;
    }

    public double VisualSimilarity(IReadOnlyList<double> a, IReadOnlyList<double> b)
    {
        if (a.Count == 0 || b.Count == 0 || a.Count != b.Count)
        {
            return 0;
        }

        // 코사인 유사도.
        double dot = 0, na = 0, nb = 0;
        for (var i = 0; i < a.Count; i++)
        {
            dot += a[i] * b[i];
            na += a[i] * a[i];
            nb += b[i] * b[i];
        }

        if (na == 0 || nb == 0)
        {
            return 0;
        }

        var cos = dot / (Math.Sqrt(na) * Math.Sqrt(nb));
        return Math.Clamp(cos, 0, 1);
    }

    private static double Weighted(double value, double weight, ref double totalWeight)
    {
        totalWeight += weight;
        return value * weight;
    }

    /// <summary>두 값이 tolerance 이내면 1에 가깝고 멀수록 0에 수렴.</summary>
    private static double Closeness(double a, double b, double tolerance)
    {
        if (a <= 0 || b <= 0)
        {
            return 0;
        }

        var diff = Math.Abs(a - b);
        return diff <= tolerance ? 1 - (diff / (tolerance * 4)) : Math.Max(0, 1 - (diff / (tolerance * 4)));
    }
}
