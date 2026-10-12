using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.AI;

/// <summary>AI 초해상(Super Resolution) 품질 복원. 후속 단계 구현.</summary>
public interface ISuperResolution
{
    /// <summary>해상도/품질을 AI로 복원한다.</summary>
    Task<RecoveryResult> UpscaleAsync(
        AiRecoveryRequest request, CancellationToken cancellationToken = default);
}
