using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 저장소 스캔 단계에서 수집하는 영상 파일 기본 정보. (요구사항 3.1)
/// </summary>
public sealed class VideoFileInfo
{
    /// <summary>원본 파일의 전체 경로.</summary>
    public required string FilePath { get; init; }

    /// <summary>파일명(확장자 포함).</summary>
    public required string FileName { get; init; }

    /// <summary>파일 크기(바이트).</summary>
    public long SizeBytes { get; init; }

    /// <summary>생성 시간.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>수정 시간.</summary>
    public DateTimeOffset ModifiedAt { get; init; }

    /// <summary>ffprobe 로 추출한 상세 메타데이터. 분석 전에는 null.</summary>
    public VideoMetadata? Metadata { get; set; }
}
