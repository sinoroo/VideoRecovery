using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 영상 건강 상태 분석 결과. 컨테이너/비디오/오디오/디코드 테스트 요약. (요구사항 4)
/// </summary>
public sealed class HealthReport
{
    /// <summary>분석 대상 파일 경로.</summary>
    public required string FilePath { get; init; }

    /// <summary>종합 손상 심각도.</summary>
    public HealthSeverity Severity { get; init; }

    /// <summary>감지된 손상 유형(복합 가능).</summary>
    public CorruptionType Corruption { get; init; }

    /// <summary>디코드 테스트로 얻은 구간 목록.</summary>
    public IReadOnlyList<DecodeSegment> DecodeSegments { get; init; } = [];

    /// <summary>프레임 이상 탐지 결과(이상 프레임만 보관 권장).</summary>
    public IReadOnlyList<FrameAnalysisResult> FrameAnomalies { get; init; } = [];

    /// <summary>오디오/비디오 동기 오차(초). 정상이면 0에 가깝다.</summary>
    public double AvSyncOffsetSeconds { get; init; }

    /// <summary>분석 수행 시각(재현성 로깅용).</summary>
    public DateTimeOffset AnalyzedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>사람이 읽을 수 있는 요약.</summary>
    public string? Summary { get; init; }
}
