using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// 복구 결과 검증기. 복구 결과물을 재분석하여 복구 전/후를 비교한다.
/// Validation Agent의 인터페이스.
/// </summary>
public interface IRecoveryValidator
{
    /// <summary>복구 결과물을 재분석하고 복구 전 리포트와 비교한다.</summary>
    Task<ValidationResult> ValidateAsync(
        RecoveryResult recovery,
        HealthReport before,
        CancellationToken cancellationToken = default);
}
