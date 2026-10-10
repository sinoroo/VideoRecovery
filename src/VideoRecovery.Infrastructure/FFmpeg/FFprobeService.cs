using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>
/// ffprobe CLI 기반 <see cref="IFFprobeService"/> 구현. (요구사항 12)
/// TODO(Phase 1): ffprobe -show_format -show_streams -print_format json 출력 파싱 구현.
/// </summary>
public sealed class FFprobeService : IFFprobeService
{
    private readonly FFmpegOptions _options;
    private readonly ProcessRunner _runner;
    private readonly ILogger<FFprobeService> _logger;

    public FFprobeService(FFmpegOptions options, ILogger<FFprobeService> logger)
    {
        _options = options;
        _logger = logger;
        _runner = new ProcessRunner(logger);
    }

    /// <inheritdoc />
    public Task<VideoMetadata> ProbeAsync(string filePath, CancellationToken cancellationToken = default)
    {
        // 의도된 CLI 호출:
        //   ffprobe -v quiet -print_format json -show_format -show_streams "<filePath>"
        // 반환된 JSON 을 VideoMetadata 로 매핑한다.
        _logger.LogInformation("Probe requested for {FilePath}", filePath);
        throw new NotImplementedException(
            "FFprobeService.ProbeAsync: ffprobe JSON 파싱을 Phase 1 에서 구현한다.");
    }
}
