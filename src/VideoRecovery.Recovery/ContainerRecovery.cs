using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// Level 1 — Container Recovery. Container/Index/Metadata/Timestamp 재구성. (요구사항 8 Level 1)
/// TODO(Phase 2): ffmpeg 리먹싱 기반 컨테이너 복구 구현.
/// </summary>
public sealed class ContainerRecovery : IContainerRecovery
{
    private const CorruptionType Handled =
        CorruptionType.ContainerCorruption | CorruptionType.IndexCorruption |
        CorruptionType.MetadataCorruption | CorruptionType.TimestampCorruption;

    private readonly IFFmpegService _ffmpeg;
    private readonly ILogger<ContainerRecovery> _logger;

    public ContainerRecovery(IFFmpegService ffmpeg, ILogger<ContainerRecovery> logger)
    {
        _ffmpeg = ffmpeg;
        _logger = logger;
    }

    /// <inheritdoc />
    public RecoveryLevel Level => RecoveryLevel.ContainerRecovery;

    /// <inheritdoc />
    public bool CanHandle(CorruptionType corruption) => (corruption & Handled) != CorruptionType.None;

    /// <inheritdoc />
    public Task<RecoveryResult> ExecuteAsync(
        RecoveryStep step,
        string sessionDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Container recovery requested in {Session}", sessionDirectory);
        throw new NotImplementedException(
            "ContainerRecovery.ExecuteAsync: 리먹싱 기반 컨테이너 복구를 Phase 2 에서 구현한다.");
    }
}
