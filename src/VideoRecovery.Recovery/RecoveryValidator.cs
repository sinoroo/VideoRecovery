using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// 복구 결과 검증기. 복구 결과물을 재분석하여 복구 전/후를 비교하고 개선 여부를 판정한다.
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

    public async Task<ValidationResult> ValidateAsync(
        RecoveryResult recovery,
        HealthReport before,
        CancellationToken cancellationToken = default)
    {
        if (!recovery.Succeeded || recovery.RecoveredPath is null)
        {
            return new ValidationResult
            {
                RecoveredPath = recovery.RecoveredPath ?? string.Empty,
                Before = before,
                Playable = false,
                FullyDecodable = false,
                Improved = false,
                ImprovementScore = 0,
                Summary = "복구 실패 — 검증 생략"
            };
        }

        var after = await _healthAnalyzer.AnalyzeAsync(recovery.RecoveredPath, cancellationToken).ConfigureAwait(false);

        var playable = after.Metadata is { ProbeFailed: false } && after.Metadata.PrimaryVideo is not null;
        var fullyDecodable = after.ErrorFrameCount == 0;

        var improvement = ComputeImprovement(before, after);
        var improved = improvement > 0;

        _logger.LogInformation(
            "복구 검증: {Path} — 재생가능={Playable}, 전체디코딩={Decodable}, 개선={Score:P0}",
            recovery.RecoveredPath, playable, fullyDecodable, improvement);

        return new ValidationResult
        {
            RecoveredPath = recovery.RecoveredPath,
            Before = before,
            After = after,
            Playable = playable,
            FullyDecodable = fullyDecodable,
            Improved = improved,
            ImprovementScore = improvement,
            Summary = BuildSummary(before, after, improvement)
        };
    }

    private static double ComputeImprovement(HealthReport before, HealthReport after)
    {
        var beforeErrors = before.ErrorFrameCount;
        var afterErrors = after.ErrorFrameCount;

        if (beforeErrors == 0)
        {
            // 원래 오류가 없었다면 오류가 늘지 않았을 때만 개선(유지)으로 본다.
            return afterErrors == 0 ? 1.0 : 0.0;
        }

        var reduced = Math.Max(0, beforeErrors - afterErrors);
        return Math.Clamp((double)reduced / beforeErrors, 0, 1);
    }

    private static string BuildSummary(HealthReport before, HealthReport after, double improvement)
        => $"오류 프레임 {before.ErrorFrameCount} → {after.ErrorFrameCount} " +
           $"(심각도 {before.Severity} → {after.Severity}), 개선 {improvement:P0}";
}
