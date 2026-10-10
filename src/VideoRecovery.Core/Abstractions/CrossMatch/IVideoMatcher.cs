using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 영상 간 유사성 분석(저장소 단위). Cross Video Analysis Agent 의 계약. (요구사항 6, 16)
/// 1단계: 메타데이터 기반 후보 선정.
/// </summary>
public interface IVideoMatcher
{
    /// <summary>
    /// 손상 영상과 유사한 reference 후보 영상을 메타데이터 기준으로 선정한다.
    /// (Codec/Resolution/FPS/Bitrate/Camera characteristics)
    /// </summary>
    Task<IReadOnlyList<ReferenceCandidate>> FindCandidatesAsync(
        VideoFileInfo source,
        IReadOnlyList<VideoFileInfo> library,
        CancellationToken cancellationToken = default);
}
