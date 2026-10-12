using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// Level 2 복구 — 손상 프레임 제거/보간 및 타임스탬프 재정렬.
/// 손상 패킷을 버리고 정상 프레임만 재디코딩·재인코딩하여 재생 가능한 영상을 만든다.
/// 원본은 보존되고 결과는 작업 영역에 생성된다.
/// </summary>
public sealed class FrameRecovery : IFrameRecovery
{
    private readonly IFFmpegService _ffmpeg;
    private readonly IWorkspaceManager _workspace;
    private readonly ILogger<FrameRecovery> _logger;

    public FrameRecovery(
        IFFmpegService ffmpeg, IWorkspaceManager workspace, ILogger<FrameRecovery> logger)
    {
        _ffmpeg = ffmpeg;
        _workspace = workspace;
        _logger = logger;
    }

    public RecoveryLevel Level => RecoveryLevel.FrameRecovery;

    public bool CanHandle(CorruptionType types)
        => (types & (CorruptionType.MissingFrame | CorruptionType.DecodeError
            | CorruptionType.CorruptedFrame | CorruptionType.DuplicateFrame
            | CorruptionType.FrameOrderError)) != 0;

    public async Task<RecoveryResult> RecoverAsync(
        string originalPath,
        string sessionPath,
        CorruptionReport corruption,
        ReferenceCandidate? reference,
        CancellationToken cancellationToken = default)
    {
        var log = new List<string>();
        var outputName = $"{Path.GetFileNameWithoutExtension(originalPath)}_frames_recovered.mp4";
        var outputPath = _workspace.GetArtifactPath(sessionPath, outputName);

        log.Add($"프레임 복구 시작: {Path.GetFileName(originalPath)} → {outputName}");
        log.Add("손상 패킷 제거 후 정상 프레임 재디코딩·재인코딩(타임스탬프 재정렬).");
        _logger.LogInformation("Frame recovery: {Original} → {Output}", originalPath, outputPath);

        var ok = await _ffmpeg
            .ReencodeDropCorruptAsync(originalPath, outputPath, cancellationToken)
            .ConfigureAwait(false);

        if (ok)
        {
            log.Add("재인코딩 성공 — 손상 프레임을 제거하고 재생 가능한 영상을 생성했습니다.");
            return new RecoveryResult
            {
                OriginalPath = originalPath,
                RecoveredPath = outputPath,
                AppliedLevel = Level,
                Succeeded = true,
                Confidence = 0.6,
                Log = log
            };
        }

        log.Add("재인코딩 실패.");
        return new RecoveryResult
        {
            OriginalPath = originalPath,
            AppliedLevel = Level,
            Succeeded = false,
            Confidence = 0,
            Log = log,
            Error = "ffmpeg 재인코딩 실패(바이너리 미설치 또는 복구 불가)"
        };
    }
}
