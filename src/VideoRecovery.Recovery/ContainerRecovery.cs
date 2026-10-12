using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// Level 1 복구 — 컨테이너/인덱스/메타데이터/타임스탬프 재구성. 스트림 복사 재먹싱으로
/// 가장 안전하게 복구한다(재인코딩 없음). 원본은 보존되고 결과는 작업 영역에 생성된다.
/// </summary>
public sealed class ContainerRecovery : IContainerRecovery
{
    private readonly IFFmpegService _ffmpeg;
    private readonly IWorkspaceManager _workspace;
    private readonly ILogger<ContainerRecovery> _logger;

    public ContainerRecovery(
        IFFmpegService ffmpeg, IWorkspaceManager workspace, ILogger<ContainerRecovery> logger)
    {
        _ffmpeg = ffmpeg;
        _workspace = workspace;
        _logger = logger;
    }

    public RecoveryLevel Level => RecoveryLevel.ContainerRecovery;

    public bool CanHandle(CorruptionType types)
        => (types & (CorruptionType.ContainerCorruption | CorruptionType.IndexCorruption
            | CorruptionType.MetadataCorruption | CorruptionType.TimestampCorruption)) != 0;

    public async Task<RecoveryResult> RecoverAsync(
        string originalPath,
        string sessionPath,
        CorruptionReport corruption,
        ReferenceCandidate? reference,
        CancellationToken cancellationToken = default)
    {
        var log = new List<string>();
        var outputName = $"{Path.GetFileNameWithoutExtension(originalPath)}_recovered.mp4";
        var outputPath = _workspace.GetArtifactPath(sessionPath, outputName);

        log.Add($"컨테이너 재먹싱 시작: {Path.GetFileName(originalPath)} → {outputName}");
        _logger.LogInformation("Container recovery: {Original} → {Output}", originalPath, outputPath);

        var ok = await _ffmpeg.RemuxAsync(originalPath, outputPath, cancellationToken).ConfigureAwait(false);

        if (ok)
        {
            log.Add("재먹싱 성공(스트림 복사).");
            return new RecoveryResult
            {
                OriginalPath = originalPath,
                RecoveredPath = outputPath,
                AppliedLevel = Level,
                Succeeded = true,
                Confidence = 0.8,
                Log = log
            };
        }

        log.Add("재먹싱 실패.");
        return new RecoveryResult
        {
            OriginalPath = originalPath,
            AppliedLevel = Level,
            Succeeded = false,
            Confidence = 0,
            Log = log,
            Error = "ffmpeg 재먹싱 실패(바이너리 미설치 또는 복구 불가)"
        };
    }
}
