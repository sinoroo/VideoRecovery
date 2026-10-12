using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Recovery;

/// <summary>
/// 복구 계획 수립기. 손상 유형을 안전한 레벨(컨테이너)부터 순서대로 매핑하여
/// 복구 단계 목록과 전체 복구 가능성, 추천 참조를 결정한다.
/// </summary>
public sealed class RecoveryPlanner : IRecoveryPlanner
{
    private readonly ILogger<RecoveryPlanner> _logger;

    public RecoveryPlanner(ILogger<RecoveryPlanner> logger) => _logger = logger;

    public RecoveryPlan CreatePlan(
        CorruptionReport corruption,
        IReadOnlyList<ReferenceCandidate> candidates)
    {
        var steps = new List<RecoveryStep>();

        // Level 1 — 컨테이너/인덱스/메타데이터/타임스탬프.
        const CorruptionType containerFlags =
            CorruptionType.ContainerCorruption | CorruptionType.IndexCorruption |
            CorruptionType.MetadataCorruption | CorruptionType.TimestampCorruption;

        if ((corruption.Types & containerFlags) != 0)
        {
            steps.Add(new RecoveryStep
            {
                Level = RecoveryLevel.ContainerRecovery,
                TargetType = corruption.Types & containerFlags,
                Description = "컨테이너/인덱스/메타데이터/타임스탬프 재구성(스트림 복사)",
                EstimatedConfidence = 0.8
            });
        }

        // Level 2 — 프레임 복구.
        const CorruptionType frameFlags =
            CorruptionType.MissingFrame | CorruptionType.DecodeError |
            CorruptionType.CorruptedFrame | CorruptionType.DuplicateFrame |
            CorruptionType.FrameOrderError;

        if ((corruption.Types & frameFlags) != 0)
        {
            steps.Add(new RecoveryStep
            {
                Level = RecoveryLevel.FrameRecovery,
                TargetType = corruption.Types & frameFlags,
                Description = "손상 프레임 제거/보간 및 타임스탬프 재정렬",
                EstimatedConfidence = 0.6
            });
        }

        // Level 3 — Cross-video 복구(혼입이거나 심각한 프레임 손상 + 참조 존재).
        var recommended = candidates.Count > 0 ? candidates[0] : null;
        var needsCross = corruption.Types.HasFlag(CorruptionType.CrossVideoContamination)
                         || (corruption.Severity >= HealthSeverity.Severe && recommended is not null);

        if (needsCross && recommended is not null)
        {
            steps.Add(new RecoveryStep
            {
                Level = RecoveryLevel.CrossVideoRecovery,
                TargetType = CorruptionType.CrossVideoContamination,
                Description = "다른 정상 영상에서 유사 프레임을 참조하여 복원",
                ReferencePath = recommended.ReferencePath,
                EstimatedConfidence = recommended.Confidence
            });
        }

        // Level 4 — AI 복구(기존 방법으로 부족한 심각한 시각 손상).
        if (corruption.Severity >= HealthSeverity.Severe)
        {
            steps.Add(new RecoveryStep
            {
                Level = RecoveryLevel.AiRecovery,
                TargetType = CorruptionType.CorruptedFrame,
                Description = "AI 기반 품질 복원(보간/초해상/아티팩트 제거) — 후속 단계",
                EstimatedConfidence = 0.4
            });
        }

        var probability = EstimateProbability(corruption, steps);

        var plan = new RecoveryPlan
        {
            FullPath = corruption.FullPath,
            Steps = steps,
            RecoveryProbability = probability,
            RecommendedReference = recommended,
            Summary = BuildSummary(corruption, steps, probability, recommended)
        };

        _logger.LogInformation(
            "복구 계획 수립: {Path} — 단계 {Steps}개, 복구가능성 {Prob:P0}",
            corruption.FullPath, steps.Count, probability);

        return plan;
    }

    private static double EstimateProbability(CorruptionReport corruption, List<RecoveryStep> steps)
    {
        if (corruption.IsHealthy)
        {
            return 1.0;
        }

        if (steps.Count == 0)
        {
            return 0.0;
        }

        // 가장 안전한(높은 신뢰도) 단계 하나가 성공할 확률을 상한으로 사용.
        var best = steps.Max(s => s.EstimatedConfidence);

        // 심각도에 따른 감쇠.
        var penalty = corruption.Severity switch
        {
            HealthSeverity.Minor => 0.95,
            HealthSeverity.Moderate => 0.85,
            HealthSeverity.Severe => 0.7,
            HealthSeverity.Unrecoverable => 0.2,
            _ => 1.0
        };

        return Math.Clamp(best * penalty, 0, 1);
    }

    private static string BuildSummary(
        CorruptionReport corruption,
        List<RecoveryStep> steps,
        double probability,
        ReferenceCandidate? recommended)
    {
        if (corruption.IsHealthy)
        {
            return "손상 없음 — 복구 불필요";
        }

        var first = steps.FirstOrDefault();
        var method = first?.Level.ToString() ?? "복구 불가";
        var refText = recommended is not null ? $", Reference: {Path.GetFileName(recommended.ReferencePath)}" : string.Empty;
        return $"추천 방법: {method}, 복구 가능성: {probability:P0}{refText}";
    }
}
