using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>
/// 프레임 이상 탐지. 프레임별 Analysis Score 를 산출한다. (요구사항 5)
/// </summary>
public interface IFrameAnalyzer
{
    /// <summary>
    /// 프레임별 이상 점수를 비동기로 열거한다. (scene cut/repeat/block artifact 등)
    /// </summary>
    IAsyncEnumerable<FrameAnalysisResult> AnalyzeFramesAsync(
        VideoFileInfo file,
        CancellationToken cancellationToken = default);
}
