using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>
/// 프레임 및 구조 손상 탐지/분류. Corruption Detection Agent 의 계약. (요구사항 7, 16)
/// </summary>
public interface ICorruptionDetector
{
    /// <summary>
    /// 건강 리포트와 프레임 분석 결과를 바탕으로 손상 유형을 분류한다.
    /// 하나의 영상에 여러 유형이 동시에 존재할 수 있다. (요구사항 7)
    /// </summary>
    Task<CorruptionReport> DetectAsync(
        VideoFileInfo file,
        HealthReport health,
        CancellationToken cancellationToken = default);
}
