using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// 복구 결과 검증. 복구된 파일을 재분석하여 복구 전/후를 비교한다.
/// Validation Agent 의 계약. (요구사항 11, 16)
/// </summary>
public interface IRecoveryValidator
{
    /// <summary>
    /// 복구 결과를 재분석하여 검증 리포트를 생성한다.
    /// </summary>
    Task<ValidationResult> ValidateAsync(
        RecoveryResult recovery,
        HealthReport? before,
        CancellationToken cancellationToken = default);
}
