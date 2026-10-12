using VideoRecovery.Core.Abstractions.AI;
using VideoRecovery.Core.Models;

namespace VideoRecovery.AI;

/// <summary>AI 초해상. Phase 5에서 실제 모델(예: Real-ESRGAN)로 구현한다.</summary>
public sealed class SuperResolution : ISuperResolution
{
    public Task<RecoveryResult> UpscaleAsync(
        AiRecoveryRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException(
            "Phase 5 — AI Super Resolution 미구현: 추론 모델 연결이 필요합니다.");
}
