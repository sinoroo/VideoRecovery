using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.CrossMatch;
using VideoRecovery.Core.Models;

namespace VideoRecovery.CrossMatch;

/// <summary>
/// 구간 단위 시간축 매칭(3단계). 연속 프레임 패턴 일치 여부 분석. (요구사항 6)
/// TODO(Phase 3): 프레임 시퀀스 유사도(시간축) 정렬/스코어링 구현.
/// </summary>
public sealed class FrameMatcher : IFrameMatcher
{
    private readonly ISimilarityEngine _similarity;
    private readonly ILogger<FrameMatcher> _logger;

    public FrameMatcher(ISimilarityEngine similarity, ILogger<FrameMatcher> logger)
    {
        _similarity = similarity;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<CrossVideoMatch>> MatchSegmentAsync(
        VideoFileInfo source,
        long sourceStartFrame,
        long sourceEndFrame,
        VideoFileInfo reference,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Segment match requested {Source}[{Start}-{End}] vs {Reference}",
            source.FilePath, sourceStartFrame, sourceEndFrame, reference.FilePath);
        throw new NotImplementedException(
            "FrameMatcher.MatchSegmentAsync: 시간축 매칭을 Phase 3 에서 구현한다.");
    }
}
