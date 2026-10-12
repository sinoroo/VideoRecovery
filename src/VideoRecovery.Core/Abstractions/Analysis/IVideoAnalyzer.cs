using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>컨테이너/비디오 스트림 분석기. Video Analysis Agent의 인터페이스.</summary>
public interface IVideoAnalyzer
{
    /// <summary>메타데이터를 수집하고 컨테이너/스트림 구조를 분석한다.</summary>
    Task<VideoMetadata> AnalyzeAsync(string path, CancellationToken cancellationToken = default);
}
