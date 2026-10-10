using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// Level 3 — Cross-Video Recovery. 다른 정상 영상의 유사 프레임으로 손상 구간 복원. (요구사항 8 Level 3)
/// TODO(Phase 4): reference 프레임 삽입/정렬 및 Confidence 산정 구현.
/// </summary>
public sealed class CrossVideoRecovery : ICrossVideoRecovery
{
    private const CorruptionType Handled =
        CorruptionType.CrossVideoContamination | CorruptionType.MissingFrame |
        CorruptionType.CorruptedFrame;

    private readonly IFFmpegService _ffmpeg;
    private readonly ILogger<CrossVideoRecovery> _logger;

    public CrossVideoRecovery(IFFmpegService ffmpeg, ILogger<CrossVideoRecovery> logger)
    {
        _ffmpeg = ffmpeg;
        _logger = logger;
    }

    /// <inheritdoc />
    public RecoveryLevel Level => RecoveryLevel.CrossVideoRecovery;

    /// <inheritdoc />
    public bool CanHandle(CorruptionType corruption) => (corruption & Handled) != CorruptionType.None;

    /// <inheritdoc />
    public Task<RecoveryResult> ExecuteAsync(
        RecoveryStep step,
        string sessionDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Cross-video recovery requested in {Session} with reference {Reference}",
            sessionDirectory, step.Reference?.FilePath);
        throw new NotImplementedException(
            "CrossVideoRecovery.ExecuteAsync: Cross-Video 복구를 Phase 4 에서 구현한다.");
    }
}
