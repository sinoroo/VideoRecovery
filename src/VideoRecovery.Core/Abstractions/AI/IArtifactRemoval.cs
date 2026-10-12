using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.AI;

/// <summary>AI 아티팩트 제거(Denoising/Deblurring/Artifact Removal). 후속 단계 구현.</summary>
public interface IArtifactRemoval
{
    /// <summary>블록 노이즈/번짐 등 아티팩트를 AI로 제거한다.</summary>
    Task<RecoveryResult> RemoveArtifactsAsync(
        AiRecoveryRequest request, CancellationToken cancellationToken = default);
}
