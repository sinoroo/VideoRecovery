using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 유사도 엔진. 메타데이터/시각 특징을 종합하여 0.0~1.0 유사도를 산출한다.
/// </summary>
public interface ISimilarityEngine
{
    /// <summary>두 메타데이터의 유사도(코덱/해상도/FPS/비트레이트 등).</summary>
    double MetadataSimilarity(VideoMetadata a, VideoMetadata b);

    /// <summary>두 특징 벡터(정규화 히스토그램 등)의 유사도.</summary>
    double VisualSimilarity(IReadOnlyList<double> a, IReadOnlyList<double> b);
}
