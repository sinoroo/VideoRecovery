using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.AI;

/// <summary>AI 프레임 보간(누락 프레임 재구성). 후속 단계 구현.</summary>
public interface IFrameInterpolation
{
    /// <summary>누락 구간을 AI로 보간한다.</summary>
    Task<RecoveryResult> InterpolateAsync(
        AiRecoveryRequest request, CancellationToken cancellationToken = default);
}
