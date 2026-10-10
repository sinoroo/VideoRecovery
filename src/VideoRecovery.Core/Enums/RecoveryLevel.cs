namespace VideoRecovery.Core.Enums;

/// <summary>
/// 복구 단계. 안전한 방법부터 단계적으로 시도한다. (요구사항 8)
/// </summary>
public enum RecoveryLevel
{
    /// <summary>복구 없음.</summary>
    None = 0,

    /// <summary>Level 1 — Container/Index/Metadata/Timestamp 재구성.</summary>
    ContainerRecovery = 1,

    /// <summary>Level 2 — 정상 프레임 보존, 손상 프레임 제거/재정렬/보간.</summary>
    FrameRecovery = 2,

    /// <summary>Level 3 — 다른 정상 영상에서 유사 프레임을 찾아 복원.</summary>
    CrossVideoRecovery = 3,

    /// <summary>Level 4 — AI 기반 복원(보간/초해상/디노이즈 등). 후속 단계.</summary>
    AiRecovery = 4,
}
