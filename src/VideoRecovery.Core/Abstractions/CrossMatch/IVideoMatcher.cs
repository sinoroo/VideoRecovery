using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 영상 간 매칭기. 메타데이터 기반(1단계)으로 손상 영상의 참조 후보를 선정한다.
/// Cross Video Analysis Agent의 상위 인터페이스.
/// </summary>
public interface IVideoMatcher
{
    /// <summary>
    /// 손상 영상의 참조로 사용할 수 있는 후보들을 메타데이터 유사도 순으로 반환한다.
    /// </summary>
    IReadOnlyList<ReferenceCandidate> FindCandidates(
        VideoMetadata target, IEnumerable<VideoMetadata> library);
}
