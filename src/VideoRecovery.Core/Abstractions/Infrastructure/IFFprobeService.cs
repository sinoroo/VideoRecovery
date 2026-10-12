using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Infrastructure;

/// <summary>
/// ffprobe 추상화. 외부 라이브러리(FFmpeg)를 Infrastructure 계층에서만
/// 호출하도록 상위 계층은 이 인터페이스에만 의존한다.
/// </summary>
public interface IFFprobeService
{
    /// <summary>지정 파일의 컨테이너/스트림 메타데이터를 수집한다.</summary>
    Task<VideoMetadata> ProbeAsync(string path, CancellationToken cancellationToken = default);
}
