using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// Level 2 복구 — 손상 프레임 제거/보간 및 타임스탬프 재정렬.
/// Phase 2에서 ffmpeg 기반 프레임 추출/재조립으로 구현한다.
/// </summary>
public sealed class FrameRecovery : IFrameRecovery
{
    public RecoveryLevel Level => RecoveryLevel.FrameRecovery;

    public bool CanHandle(CorruptionType types)
        => (types & (CorruptionType.MissingFrame | CorruptionType.DecodeError
            | CorruptionType.CorruptedFrame | CorruptionType.DuplicateFrame
            | CorruptionType.FrameOrderError)) != 0;

    public Task<RecoveryResult> RecoverAsync(
        string originalPath,
        string sessionPath,
        CorruptionReport corruption,
        ReferenceCandidate? reference,
        CancellationToken cancellationToken = default)
        // Phase 2: 정상 프레임 보존 → 손상 프레임 제거 → 타임스탬프 재정렬 → 재조립.
        => throw new NotImplementedException(
            "Phase 2 — Frame Recovery 미구현: 손상 프레임 제거/보간 로직을 추가해야 합니다.");
}
