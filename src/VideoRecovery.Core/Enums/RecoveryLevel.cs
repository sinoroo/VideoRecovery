namespace VideoRecovery.Core.Enums;

/// <summary>
/// 복구 단계. 가장 안전한 방법(낮은 레벨)부터 순차적으로 시도한다.
/// </summary>
public enum RecoveryLevel
{
    /// <summary>복구 불필요 또는 미정.</summary>
    None = 0,

    /// <summary>Level 1 — 컨테이너/인덱스/메타데이터/타임스탬프 재구성.</summary>
    ContainerRecovery = 1,

    /// <summary>Level 2 — 정상 프레임 보존, 손상 프레임 제거/보간.</summary>
    FrameRecovery = 2,

    /// <summary>Level 3 — 다른 정상 영상에서 유사 프레임 복원.</summary>
    CrossVideoRecovery = 3,

    /// <summary>Level 4 — AI 기반 품질 복원(후속 단계).</summary>
    AiRecovery = 4
}
