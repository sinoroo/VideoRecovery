namespace VideoRecovery.Core.Models;

/// <summary>
/// 스캐너가 수집하는 영상 파일의 기본 정보(디코딩 전).
/// 원본을 수정하지 않으므로 모든 값은 읽기 전용 스냅샷이다.
/// </summary>
public sealed class VideoFileInfo
{
    /// <summary>절대 경로.</summary>
    public required string FullPath { get; init; }

    /// <summary>파일명(확장자 포함).</summary>
    public required string FileName { get; init; }

    /// <summary>파일 크기(바이트).</summary>
    public long SizeBytes { get; init; }

    /// <summary>생성 시각.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>수정 시각.</summary>
    public DateTimeOffset ModifiedAt { get; init; }

    /// <summary>컨테이너 추정(확장자 기반, 예: mp4/mov/mkv).</summary>
    public string Container { get; init; } = string.Empty;
}
