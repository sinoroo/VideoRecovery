namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// Level 1 — Container Recovery. 가장 안전한 복구.
/// Container/Index/Metadata/Timestamp/Stream 재구성. (요구사항 8 Level 1)
/// </summary>
public interface IContainerRecovery : IRecoveryStrategy
{
}
