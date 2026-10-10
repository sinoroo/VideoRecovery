namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// Level 2 — Frame Recovery. 정상 프레임 보존, 손상 프레임 제거/재정렬/보간. (요구사항 8 Level 2)
/// </summary>
public interface IFrameRecovery : IRecoveryStrategy
{
}
