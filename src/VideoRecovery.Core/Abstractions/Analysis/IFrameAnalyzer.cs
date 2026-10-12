using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Analysis;

/// <summary>프레임 이상 탐지기. 프레임별 Analysis Score를 산출한다.</summary>
public interface IFrameAnalyzer
{
    /// <summary>프레임 단위 이상(급변/중복/깨짐 등)을 탐지한다.</summary>
    Task<FrameAnalysisResult> AnalyzeAsync(
        string path, CancellationToken cancellationToken = default);
}
