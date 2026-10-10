using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 프레임 이상 탐지. 프레임별 Analysis Score 산출. (요구사항 5)
/// TODO(Phase 1): OpenCV 기반 scene-cut/repeat/block-artifact 점수화 구현.
/// </summary>
public sealed class FrameAnalyzer : IFrameAnalyzer
{
    private readonly ILogger<FrameAnalyzer> _logger;

    public FrameAnalyzer(ILogger<FrameAnalyzer> logger) => _logger = logger;

    /// <inheritdoc />
    public async IAsyncEnumerable<FrameAnalysisResult> AnalyzeFramesAsync(
        VideoFileInfo file,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Frame analysis requested for {FilePath}", file.FilePath);
        await Task.CompletedTask.ConfigureAwait(false);
        throw new NotImplementedException(
            "FrameAnalyzer.AnalyzeFramesAsync: 프레임 이상 점수화를 Phase 1 에서 구현한다.");
#pragma warning disable CS0162
        yield break;
#pragma warning restore CS0162
    }
}
