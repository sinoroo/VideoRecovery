using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Recovery;

/// <summary>
/// 복구 전략 공통 계약. 각 Recovery Level 구현이 이 인터페이스를 따른다.
/// 하나의 전략이 실패하면 다른 전략으로 교체할 수 있어야 한다. (요구사항 17-6)
/// </summary>
public interface IRecoveryStrategy
{
    /// <summary>이 전략이 담당하는 복구 단계.</summary>
    RecoveryLevel Level { get; }

    /// <summary>주어진 손상 유형을 이 전략으로 처리할 수 있는지 여부.</summary>
    bool CanHandle(CorruptionType corruption);

    /// <summary>
    /// 복구를 실행한다. 원본은 수정하지 않고 작업 영역에 결과물을 생성한다. (요구사항 2.1)
    /// </summary>
    Task<RecoveryResult> ExecuteAsync(
        RecoveryStep step,
        string sessionDirectory,
        CancellationToken cancellationToken = default);
}
