using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.CrossMatch;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Models;

namespace VideoRecovery.CrossMatch;

/// <summary>
/// 프레임/구간 매칭기(2·3단계). 손상 영상의 구간 프레임을 추출하여 후보 영상들의
/// 프레임과 시각(pHash) + 시간축(연속 패턴) 유사도로 비교한다.
/// 연속 프레임 패턴까지 일치하면 Cross-video contamination 가능성이 높다.
/// </summary>
public sealed class FrameMatcher : IFrameMatcher
{
    private readonly IFFmpegService _ffmpeg;
    private readonly IFeatureMatcher _features;
    private readonly ILogger<FrameMatcher> _logger;

    /// <summary>매치로 인정하는 최소 종합 신뢰도.</summary>
    public double MinimumConfidence { get; set; } = 0.75;

    public FrameMatcher(IFFmpegService ffmpeg, IFeatureMatcher features, ILogger<FrameMatcher> logger)
    {
        _ffmpeg = ffmpeg;
        _features = features;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CrossVideoMatch>> MatchSegmentAsync(
        string sourcePath,
        long sourceStartFrame,
        int length,
        IEnumerable<string> referencePaths,
        CancellationToken cancellationToken = default)
    {
        var matches = new List<CrossVideoMatch>();

        // 손상 구간의 프레임 해시를 추출.
        var sourceHashes = await ExtractHashesAsync(sourcePath, sourceStartFrame, length, cancellationToken)
            .ConfigureAwait(false);
        if (sourceHashes.Count == 0)
        {
            return matches;
        }

        foreach (var reference in referencePaths)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // 참조 영상의 동일 프레임 위치부터 비교(스캔 범위는 후속 단계에서 확장).
            var refHashes = await ExtractHashesAsync(reference, sourceStartFrame, length, cancellationToken)
                .ConfigureAwait(false);
            if (refHashes.Count == 0)
            {
                continue;
            }

            var (visual, temporal) = Compare(sourceHashes, refHashes);
            var confidence = (visual * 0.6) + (temporal * 0.4);

            if (confidence >= MinimumConfidence)
            {
                matches.Add(new CrossVideoMatch
                {
                    SourcePath = sourcePath,
                    ReferencePath = reference,
                    SourceStartFrame = sourceStartFrame,
                    ReferenceStartFrame = sourceStartFrame,
                    Length = Math.Min(sourceHashes.Count, refHashes.Count),
                    VisualSimilarity = visual,
                    TemporalSimilarity = temporal,
                    Confidence = confidence
                });
            }
        }

        var ordered = matches.OrderByDescending(m => m.Confidence).ToList();
        _logger.LogInformation(
            "구간 매칭: {Source}#{Start}+{Len} → 매치 {Count}건",
            sourcePath, sourceStartFrame, length, ordered.Count);

        return ordered;
    }

    private async Task<List<ulong>> ExtractHashesAsync(
        string path, long startFrame, int length, CancellationToken cancellationToken)
    {
        var hashes = new List<ulong>(length);
        for (var i = 0; i < length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var image = await _ffmpeg.ExtractFrameAsync(path, startFrame + i, cancellationToken).ConfigureAwait(false);
            if (image.Length == 0)
            {
                break;
            }

            hashes.Add(_features.ComputePerceptualHash(image));
        }

        return hashes;
    }

    /// <summary>시각 유사도(프레임별 평균)와 시간축 유사도(연속 패턴 일치율)를 계산.</summary>
    private (double Visual, double Temporal) Compare(List<ulong> source, List<ulong> reference)
    {
        var n = Math.Min(source.Count, reference.Count);
        if (n == 0)
        {
            return (0, 0);
        }

        double visualSum = 0;
        var matchedConsecutive = 0;
        var bestRun = 0;
        var run = 0;

        for (var i = 0; i < n; i++)
        {
            var sim = _features.HashSimilarity(source[i], reference[i]);
            visualSum += sim;

            if (sim >= 0.9)
            {
                run++;
                matchedConsecutive++;
                bestRun = Math.Max(bestRun, run);
            }
            else
            {
                run = 0;
            }
        }

        var visual = visualSum / n;
        var temporal = (double)bestRun / n; // 가장 긴 연속 일치 구간 비율.
        return (visual, temporal);
    }
}
