using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// Level 2 — Frame Recovery. 정상 프레임 보존, 손상 프레임 제거/재정렬/보간. (요구사항 8 Level 2)
/// TODO(Phase 2): 손상 프레임 제거 + Timestamp 재정렬 구현.
/// </summary>
public sealed class FrameRecovery : IFrameRecovery
{
    private const CorruptionType Handled =
        CorruptionType.MissingFrame | CorruptionType.DecodeError |
        CorruptionType.CorruptedFrame | CorruptionType.DuplicateFrame |
        CorruptionType.FrameOrderError;

    private readonly IFFmpegService _ffmpeg;
    private readonly ILogger<FrameRecovery> _logger;

    public FrameRecovery(IFFmpegService ffmpeg, ILogger<FrameRecovery> logger)
    {
        _ffmpeg = ffmpeg;
        _logger = logger;
    }

    /// <inheritdoc />
    public RecoveryLevel Level => RecoveryLevel.FrameRecovery;

    /// <inheritdoc />
    public bool CanHandle(CorruptionType corruption) => (corruption & Handled) != CorruptionType.None;

    /// <inheritdoc />
    public Task<RecoveryResult> ExecuteAsync(
        RecoveryStep step,
        string sessionDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Frame recovery requested in {Session}", sessionDirectory);
        throw new NotImplementedException(
            "FrameRecovery.ExecuteAsync: 프레임 복구를 Phase 2 에서 구현한다.");
    }
}
