using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// 복구 결과 검증. 복구된 파일을 재분석하여 복구 전/후를 비교한다. Validation Agent. (요구사항 11, 16)
/// TODO(Phase 2): 재분석 결과로 개선율 및 통과 여부 산정 구현.
/// </summary>
public sealed class RecoveryValidator : IRecoveryValidator
{
    private readonly IHealthAnalyzer _healthAnalyzer;
    private readonly ILogger<RecoveryValidator> _logger;

    public RecoveryValidator(IHealthAnalyzer healthAnalyzer, ILogger<RecoveryValidator> logger)
    {
        _healthAnalyzer = healthAnalyzer;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(
        RecoveryResult recovery,
        HealthReport? before,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validation requested for {FilePath}", recovery.RecoveredFilePath);
        throw new NotImplementedException(
            "RecoveryValidator.ValidateAsync: 복구 결과 재분석/비교를 Phase 2 에서 구현한다.");
    }
}
