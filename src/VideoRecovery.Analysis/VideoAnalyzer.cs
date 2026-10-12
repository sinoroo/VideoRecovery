using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 컨테이너/비디오 스트림 분석기. ffprobe 메타데이터 수집을 담당한다.
/// </summary>
public sealed class VideoAnalyzer : IVideoAnalyzer
{
    private readonly IFFprobeService _ffprobe;
    private readonly ILogger<VideoAnalyzer> _logger;

    public VideoAnalyzer(IFFprobeService ffprobe, ILogger<VideoAnalyzer> logger)
    {
        _ffprobe = ffprobe;
        _logger = logger;
    }

    public async Task<VideoMetadata> AnalyzeAsync(string path, CancellationToken cancellationToken = default)
    {
        var metadata = await _ffprobe.ProbeAsync(path, cancellationToken).ConfigureAwait(false);

        if (metadata.ProbeFailed)
        {
            _logger.LogWarning("메타데이터 분석 실패: {Path} ({Error})", path, metadata.ProbeError);
        }

        return metadata;
    }
}
