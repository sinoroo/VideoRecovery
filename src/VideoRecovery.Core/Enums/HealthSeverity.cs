namespace VideoRecovery.Core.Enums;

/// <summary>영상 건강 상태의 심각도.</summary>
public enum HealthSeverity
{
    /// <summary>정상.</summary>
    Healthy = 0,

    /// <summary>경미한 이상(재생에는 영향 적음).</summary>
    Minor = 1,

    /// <summary>중간 수준 손상.</summary>
    Moderate = 2,

    /// <summary>심각한 손상.</summary>
    Severe = 3,

    /// <summary>분석 불가 또는 복구 불가능 추정.</summary>
    Unrecoverable = 4
}
