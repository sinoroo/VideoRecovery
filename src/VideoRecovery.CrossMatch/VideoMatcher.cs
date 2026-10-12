using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.CrossMatch;
using VideoRecovery.Core.Models;

namespace VideoRecovery.CrossMatch;

/// <summary>
/// 영상 간 매칭기(1단계: 메타데이터 기반 후보 선정). 손상 영상과 코덱/해상도/FPS가
/// 유사한 정상 영상을 참조 후보로 추천한다.
/// </summary>
public sealed class VideoMatcher : IVideoMatcher
{
    private readonly ISimilarityEngine _similarity;
    private readonly ILogger<VideoMatcher> _logger;

    /// <summary>후보로 인정하는 최소 메타데이터 유사도.</summary>
    public double MinimumSimilarity { get; set; } = 0.5;

    public VideoMatcher(ISimilarityEngine similarity, ILogger<VideoMatcher> logger)
    {
        _similarity = similarity;
        _logger = logger;
    }

    public IReadOnlyList<ReferenceCandidate> FindCandidates(
        VideoMetadata target, IEnumerable<VideoMetadata> library)
    {
        var candidates = new List<ReferenceCandidate>();

        foreach (var other in library)
        {
            if (other.ProbeFailed
                || string.Equals(other.FullPath, target.FullPath, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var sim = _similarity.MetadataSimilarity(target, other);
            if (sim < MinimumSimilarity)
            {
                continue;
            }

            candidates.Add(new ReferenceCandidate
            {
                ReferencePath = other.FullPath,
                MetadataSimilarity = sim,
                VisualSimilarity = 0, // 시각 유사도는 FrameMatcher 단계에서 보강.
                Confidence = sim,
                Reason = $"메타데이터 유사도 {sim:P0}"
            });
        }

        var ordered = candidates
            .OrderByDescending(c => c.Confidence)
            .ToList();

        _logger.LogInformation(
            "참조 후보 선정: {Target} → {Count}개", target.FullPath, ordered.Count);

        return ordered;
    }
}
