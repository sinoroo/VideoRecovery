using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// 복구 전략 수립. 손상 유형별로 복구 단계 순서를 결정한다. Recovery Planning Agent. (요구사항 9, 16)
/// TODO(Phase 2): 손상 유형 → Recovery Level 매핑 규칙 및 복구 가능성 추정 구현.
/// </summary>
public sealed class RecoveryPlanner : IRecoveryPlanner
{
    private readonly ILogger<RecoveryPlanner> _logger;

    public RecoveryPlanner(ILogger<RecoveryPlanner> logger) => _logger = logger;

    /// <inheritdoc />
    public Task<RecoveryPlan> CreatePlanAsync(
        VideoFileInfo file,
        CorruptionReport corruption,
        IReadOnlyList<ReferenceCandidate> references,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Recovery plan requested for {FilePath} (corruption={Corruption})",
            file.FilePath, corruption.Aggregate);
        throw new NotImplementedException(
            "RecoveryPlanner.CreatePlanAsync: 복구 전략 결정을 Phase 2 에서 구현한다.");
    }
}
