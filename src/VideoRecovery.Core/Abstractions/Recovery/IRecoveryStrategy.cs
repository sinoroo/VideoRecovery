using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// 복구 전략 공통 인터페이스. 각 복구 레벨은 이 계약을 구현하며,
/// 하나가 실패하면 Planner가 다른 전략을 시도할 수 있다.
/// </summary>
public interface IRecoveryStrategy
{
    /// <summary>이 전략이 담당하는 복구 레벨.</summary>
    RecoveryLevel Level { get; }

    /// <summary>주어진 손상 유형을 이 전략이 처리할 수 있는지 여부.</summary>
    bool CanHandle(CorruptionType types);

    /// <summary>복구를 실행한다(원본 미변경, 결과물은 작업 영역에 생성).</summary>
    Task<RecoveryResult> RecoverAsync(
        string originalPath,
        string sessionPath,
        CorruptionReport corruption,
        ReferenceCandidate? reference,
        CancellationToken cancellationToken = default);
}
