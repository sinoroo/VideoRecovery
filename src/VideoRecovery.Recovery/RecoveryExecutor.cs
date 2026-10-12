using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// <see cref="IRecoveryExecutor"/> 구현. 복구 계획의 단계를 안전한 레벨(컨테이너)부터
/// 순서대로 시도하며, 성공한 결과를 검증한다. 미구현 전략(NotImplemented)은 건너뛰고
/// 다음 단계로 넘어간다. 원본은 변경하지 않고 모든 산출물은 격리 세션에 생성된다.
/// </summary>
public sealed class RecoveryExecutor : IRecoveryExecutor
{
    private readonly IReadOnlyList<IRecoveryStrategy> _strategies;
    private readonly IWorkspaceManager _workspace;
    private readonly IRecoveryValidator _validator;
    private readonly ILogger<RecoveryExecutor> _logger;

    public RecoveryExecutor(
        IEnumerable<IRecoveryStrategy> strategies,
        IWorkspaceManager workspace,
        IRecoveryValidator validator,
        ILogger<RecoveryExecutor> logger)
    {
        _strategies = strategies.ToList();
        _workspace = workspace;
        _validator = validator;
        _logger = logger;
    }

    public async Task<RecoveryOutcome> ExecuteAsync(
        string originalPath,
        HealthReport before,
        CorruptionReport corruption,
        RecoveryPlan plan,
        CancellationToken cancellationToken = default)
    {
        var attempts = new List<string>();

        if (plan.Steps.Count == 0)
        {
            return new RecoveryOutcome
            {
                OriginalPath = originalPath,
                Attempts = new[] { "복구 단계 없음 — 손상이 없거나 복구 전략을 찾지 못했습니다." },
                Summary = corruption.IsHealthy ? "복구 불필요(정상)" : "복구 불가 — 적용 가능한 전략 없음"
            };
        }

        var session = _workspace.CreateSession(originalPath);
        _logger.LogInformation("복구 세션 생성: {Session}", session);

        RecoveryResult? bestResult = null;
        ValidationResult? bestValidation = null;

        foreach (var step in plan.Steps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var strategy = _strategies.FirstOrDefault(s => s.Level == step.Level);
            if (strategy is null)
            {
                attempts.Add($"[{step.Level}] 등록된 전략 없음 — 건너뜀");
                continue;
            }

            try
            {
                var result = await strategy
                    .RecoverAsync(originalPath, session, corruption, plan.RecommendedReference, cancellationToken)
                    .ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    attempts.Add($"[{step.Level}] 실패: {result.Error ?? "알 수 없음"}");
                    bestResult ??= result;
                    continue;
                }

                var validation = await _validator
                    .ValidateAsync(result, before, cancellationToken)
                    .ConfigureAwait(false);

                attempts.Add(
                    $"[{step.Level}] 성공 (신뢰도 {result.Confidence:P0}) — " +
                    $"재생가능={validation.Playable}, 개선={validation.ImprovementScore:P0}");

                bestResult = result;
                bestValidation = validation;

                // 재생 가능하고 추가 오류가 없으면 더 공격적인 단계를 시도하지 않는다.
                if (validation.Playable && validation.FullyDecodable)
                {
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NotImplementedException)
            {
                attempts.Add($"[{step.Level}] 미구현 전략 — 건너뜀");
                _logger.LogInformation("복구 전략 미구현, 건너뜀: {Level}", step.Level);
            }
            catch (Exception ex)
            {
                attempts.Add($"[{step.Level}] 예외: {ex.Message}");
                _logger.LogError(ex, "복구 단계 실패: {Level}", step.Level);
            }
        }

        var succeeded = bestResult is { Succeeded: true };
        var summary = BuildSummary(bestResult, bestValidation);

        _logger.LogInformation(
            "복구 실행 완료: {Path} — 성공={Succeeded}, {Summary}",
            originalPath, succeeded, summary);

        return new RecoveryOutcome
        {
            OriginalPath = originalPath,
            SessionPath = session,
            Recovery = bestResult,
            Validation = bestValidation,
            Attempts = attempts,
            Summary = summary
        };
    }

    private static string BuildSummary(RecoveryResult? result, ValidationResult? validation)
    {
        if (result is not { Succeeded: true })
        {
            return "복구 실패 — 재생 가능한 결과물을 생성하지 못했습니다.";
        }

        var method = result.AppliedLevel.ToString();
        if (validation is not null)
        {
            return $"{method} 복구 성공 — 재생가능={(validation.Playable ? "예" : "아니오")}, " +
                   $"개선 {validation.ImprovementScore:P0}";
        }

        return $"{method} 복구 성공";
    }
}
