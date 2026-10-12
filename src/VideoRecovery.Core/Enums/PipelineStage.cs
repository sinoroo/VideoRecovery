namespace VideoRecovery.Core.Enums;

/// <summary>진단/복구 파이프라인의 단계.</summary>
public enum PipelineStage
{
    /// <summary>저장소 스캔.</summary>
    Scan = 0,

    /// <summary>메타데이터/건강 분석.</summary>
    Analysis = 1,

    /// <summary>손상 유형 분류.</summary>
    CorruptionClassification = 2,

    /// <summary>교차 영상 분석.</summary>
    CrossVideoAnalysis = 3,

    /// <summary>복구 전략 수립.</summary>
    RecoveryPlanning = 4,

    /// <summary>복구 실행.</summary>
    Recovery = 5,

    /// <summary>복구 결과 검증.</summary>
    Validation = 6
}
