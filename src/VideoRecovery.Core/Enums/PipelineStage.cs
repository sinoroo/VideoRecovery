namespace VideoRecovery.Core.Enums;

/// <summary>
/// 분석/복구 파이프라인 단계. 로깅 및 진행 상태 표시에 사용한다. (요구사항 14, 18)
/// </summary>
public enum PipelineStage
{
    /// <summary>저장소 스캔.</summary>
    Scan = 0,

    /// <summary>메타데이터 분석.</summary>
    Metadata = 1,

    /// <summary>디코드 테스트.</summary>
    DecodeTest = 2,

    /// <summary>프레임 이상 탐지.</summary>
    FrameAnalysis = 3,

    /// <summary>손상 유형 분류.</summary>
    CorruptionClassification = 4,

    /// <summary>Cross-Video 비교.</summary>
    CrossVideoAnalysis = 5,

    /// <summary>복구 전략 수립.</summary>
    RecoveryPlanning = 6,

    /// <summary>복구 실행.</summary>
    Recovery = 7,

    /// <summary>복구 결과 검증.</summary>
    Validation = 8,
}
