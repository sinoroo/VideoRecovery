using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 단일 영상의 건강 상태 종합 리포트. 컨테이너/비디오/오디오/디코드
/// 분석 결과를 하나로 묶는다. 원본은 변경하지 않는 읽기 전용 산출물이다.
/// </summary>
public sealed class HealthReport
{
    /// <summary>대상 파일 경로.</summary>
    public required string FullPath { get; init; }

    /// <summary>메타데이터(ffprobe).</summary>
    public VideoMetadata? Metadata { get; init; }

    /// <summary>디코드 테스트 구간(정상/오류).</summary>
    public IReadOnlyList<DecodeSegment> DecodeSegments { get; init; }
        = Array.Empty<DecodeSegment>();

    /// <summary>프레임 이상 탐지 결과.</summary>
    public FrameAnalysisResult? FrameAnalysis { get; init; }

    /// <summary>오디오/비디오 동기화 오차(초). null이면 미측정.</summary>
    public double? AvSyncOffsetSeconds { get; init; }

    /// <summary>종합 심각도.</summary>
    public HealthSeverity Severity { get; init; } = HealthSeverity.Healthy;

    /// <summary>사람이 읽을 수 있는 요약.</summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>오류 구간 개수.</summary>
    public int ErrorSegmentCount => DecodeSegments.Count(s => s.IsError);

    /// <summary>오류 프레임 총 개수.</summary>
    public long ErrorFrameCount => DecodeSegments.Where(s => s.IsError).Sum(s => s.FrameLength);
}
