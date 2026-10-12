using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// 복구 실행 오케스트레이터. 복구 계획의 단계를 가장 안전한 레벨부터 순서대로 시도하고,
/// 성공한 결과를 검증하여 최종 <see cref="RecoveryOutcome"/>을 반환한다.
/// 원본은 절대 변경하지 않으며 모든 산출물은 격리된 작업 세션에 생성된다.
/// </summary>
public interface IRecoveryExecutor
{
    /// <summary>복구 계획에 따라 복구를 실행하고 검증한다.</summary>
    /// <param name="originalPath">원본 파일 경로(변경되지 않음).</param>
    /// <param name="before">복구 전 건강 리포트(검증 비교 기준).</param>
    /// <param name="corruption">손상 분류.</param>
    /// <param name="plan">복구 계획(정렬된 단계 목록).</param>
    /// <param name="cancellationToken">취소 토큰.</param>
    Task<RecoveryOutcome> ExecuteAsync(
        string originalPath,
        HealthReport before,
        CorruptionReport corruption,
        RecoveryPlan plan,
        CancellationToken cancellationToken = default);
}
