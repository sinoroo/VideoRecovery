using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Infrastructure;

/// <summary>디코드 테스트 중 보고되는 단일 오류.</summary>
public sealed class DecodeError
{
    /// <summary>오류 발생 프레임(알 수 없으면 null).</summary>
    public long? FrameIndex { get; init; }

    /// <summary>오류 발생 시각(초, 알 수 없으면 null).</summary>
    public double? TimeSeconds { get; init; }

    /// <summary>ffmpeg가 보고한 원본 메시지.</summary>
    public string Message { get; init; } = string.Empty;
}

/// <summary>ffmpeg CLI 추상화. 디코드 테스트/프레임 추출/재먹싱 등을 수행한다.</summary>
public interface IFFmpegService
{
    /// <summary>
    /// 전체 영상을 실제 디코딩하며 오류 위치를 수집한다(원본 미변경).
    /// </summary>
    Task<IReadOnlyList<DecodeError>> DecodeTestAsync(
        string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// 지정 프레임 인덱스의 썸네일/프레임을 추출하여 바이트로 반환한다.
    /// 프레임 유사도 분석에 사용된다.
    /// </summary>
    Task<byte[]> ExtractFrameAsync(
        string path, long frameIndex, CancellationToken cancellationToken = default);

    /// <summary>
    /// 컨테이너를 재먹싱(remux)하여 작업 영역에 새 파일을 만든다(스트림 복사).
    /// 원본은 변경하지 않는다.
    /// </summary>
    Task<bool> RemuxAsync(
        string inputPath, string outputPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// 손상 패킷/프레임을 버리고 정상 프레임만 재디코딩·재인코딩하여 새 파일을 만든다.
    /// 타임스탬프를 재정렬하며(Level 2 프레임 복구) 원본은 변경하지 않는다.
    /// </summary>
    Task<bool> ReencodeDropCorruptAsync(
        string inputPath, string outputPath, CancellationToken cancellationToken = default);
}
