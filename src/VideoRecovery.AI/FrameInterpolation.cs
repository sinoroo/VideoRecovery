using VideoRecovery.Core.Abstractions.AI;
using VideoRecovery.Core.Models;

namespace VideoRecovery.AI;

/// <summary>AI 프레임 보간. Phase 5에서 실제 모델(예: RIFE/FILM)로 구현한다.</summary>
public sealed class FrameInterpolation : IFrameInterpolation
{
    public Task<RecoveryResult> InterpolateAsync(
        AiRecoveryRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException(
            "Phase 5 — AI Frame Interpolation 미구현: 추론 모델 연결이 필요합니다.");
}
