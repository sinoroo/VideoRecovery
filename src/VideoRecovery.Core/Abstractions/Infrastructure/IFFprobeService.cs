using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Infrastructure;

/// <summary>
/// ffprobe CLI 래퍼. 컨테이너/스트림 메타데이터를 추출한다.
/// 외부 라이브러리는 Infrastructure 계층에서만 추상화한다. (요구사항 12, 17-9)
/// </summary>
public interface IFFprobeService
{
    /// <summary>
    /// 지정한 영상 파일의 메타데이터를 추출한다. 원본은 읽기 전용으로만 접근한다.
    /// </summary>
    Task<VideoMetadata> ProbeAsync(string filePath, CancellationToken cancellationToken = default);
}
