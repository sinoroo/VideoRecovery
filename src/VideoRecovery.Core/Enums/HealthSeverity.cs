namespace VideoRecovery.Core.Enums;

/// <summary>
/// 손상 심각도 및 영상 건강 상태 등급. (요구사항 4, 10)
/// </summary>
public enum HealthSeverity
{
    /// <summary>알 수 없음 / 미분석.</summary>
    Unknown = 0,

    /// <summary>정상.</summary>
    Healthy = 1,

    /// <summary>경미한 손상. 재생에 거의 영향 없음.</summary>
    Minor = 2,

    /// <summary>중간 손상. 일부 구간 복구 필요.</summary>
    Moderate = 3,

    /// <summary>심각한 손상. 적극적 복구 필요.</summary>
    Severe = 4,

    /// <summary>복구 불가로 추정.</summary>
    Unrecoverable = 5,
}
