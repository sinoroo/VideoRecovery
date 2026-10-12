using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// Level 3 복구 — 다른 정상 영상에서 유사 프레임을 참조하여 손상 구간을 복원한다.
/// Phase 4에서 FrameMatcher 결과를 사용해 reference 프레임을 삽입하는 로직을 구현한다.
/// </summary>
public sealed class CrossVideoRecovery : ICrossVideoRecovery
{
    public RecoveryLevel Level => RecoveryLevel.CrossVideoRecovery;

    public bool CanHandle(CorruptionType types)
        => types.HasFlag(CorruptionType.CrossVideoContamination)
           || types.HasFlag(CorruptionType.CorruptedFrame)
           || types.HasFlag(CorruptionType.MissingFrame);

    public Task<RecoveryResult> RecoverAsync(
        string originalPath,
        string sessionPath,
        CorruptionReport corruption,
        ReferenceCandidate? reference,
        CancellationToken cancellationToken = default)
        // Phase 4: reference 영상에서 매칭 구간 프레임을 추출하여 손상 구간에 삽입.
        => throw new NotImplementedException(
            "Phase 4 — Cross-Video Recovery 미구현: reference 프레임 복원 로직을 추가해야 합니다.");
}
