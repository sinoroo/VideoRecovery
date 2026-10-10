using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>
/// 컨테이너/비디오 스트림 분석. Video Analysis Agent 의 계약. (요구사항 4, 16)
/// </summary>
public interface IVideoAnalyzer
{
    /// <summary>
    /// 메타데이터와 디코드 테스트를 수행하여 영상 건강 리포트를 생성한다.
    /// 원본은 수정하지 않는다. (요구사항 2.1, 4)
    /// </summary>
    Task<HealthReport> AnalyzeAsync(
        VideoFileInfo file,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);
}
