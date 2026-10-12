using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 프레임/구간 매칭기. 손상 구간이 다른 영상의 구간과 일치하는지(시각+시간축)
/// 비교하여 Cross-video contamination을 탐지한다(2·3단계).
/// </summary>
public interface IFrameMatcher
{
    /// <summary>
    /// 손상 영상의 특정 구간과 후보 영상들을 비교하여 매치를 반환한다.
    /// </summary>
    Task<IReadOnlyList<CrossVideoMatch>> MatchSegmentAsync(
        string sourcePath,
        long sourceStartFrame,
        int length,
        IEnumerable<string> referencePaths,
        CancellationToken cancellationToken = default);
}
