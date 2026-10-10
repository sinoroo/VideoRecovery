using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.CrossMatch;
using VideoRecovery.Core.Models;

namespace VideoRecovery.CrossMatch;

/// <summary>
/// 메타데이터 기반 reference 후보 선정(1단계). Cross Video Analysis Agent. (요구사항 6, 16)
/// TODO(Phase 3): Codec/Resolution/FPS/Bitrate/Camera 특성 기반 후보 스코어링.
/// </summary>
public sealed class VideoMatcher : IVideoMatcher
{
    private readonly ILogger<VideoMatcher> _logger;

    public VideoMatcher(ILogger<VideoMatcher> logger) => _logger = logger;

    /// <inheritdoc />
    public Task<IReadOnlyList<ReferenceCandidate>> FindCandidatesAsync(
        VideoFileInfo source,
        IReadOnlyList<VideoFileInfo> library,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Candidate search requested for {Source} against {Count} library files",
            source.FilePath, library.Count);
        throw new NotImplementedException(
            "VideoMatcher.FindCandidatesAsync: 메타데이터 후보 선정을 Phase 3 에서 구현한다.");
    }
}
