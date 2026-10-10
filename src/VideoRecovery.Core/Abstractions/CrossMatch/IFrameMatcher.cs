using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 프레임/구간 단위 시간축 매칭. (요구사항 6 - 3단계 Temporal Similarity)
/// 연속적인 프레임 패턴까지 일치하는지 분석한다.
/// </summary>
public interface IFrameMatcher
{
    /// <summary>
    /// 소스 영상의 의심 구간과 reference 영상 간 시간축 매칭을 수행한다.
    /// </summary>
    Task<IReadOnlyList<CrossVideoMatch>> MatchSegmentAsync(
        VideoFileInfo source,
        long sourceStartFrame,
        long sourceEndFrame,
        VideoFileInfo reference,
        CancellationToken cancellationToken = default);
}
