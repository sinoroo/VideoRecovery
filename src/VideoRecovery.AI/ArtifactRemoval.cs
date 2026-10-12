using VideoRecovery.Core.Abstractions.AI;
using VideoRecovery.Core.Models;

namespace VideoRecovery.AI;

/// <summary>AI 아티팩트 제거(Denoising/Deblurring). Phase 5에서 실제 모델로 구현한다.</summary>
public sealed class ArtifactRemoval : IArtifactRemoval
{
    public Task<RecoveryResult> RemoveArtifactsAsync(
        AiRecoveryRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException(
            "Phase 5 — AI Artifact Removal 미구현: 추론 모델 연결이 필요합니다.");
}
