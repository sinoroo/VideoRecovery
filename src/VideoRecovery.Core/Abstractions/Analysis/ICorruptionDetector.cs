using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>손상 유형 분류기. Corruption Detection Agent의 인터페이스.</summary>
public interface ICorruptionDetector
{
    /// <summary>건강 리포트를 바탕으로 손상 유형을 분류한다.</summary>
    CorruptionReport Classify(HealthReport health);
}
