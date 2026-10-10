using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.AI;

namespace VideoRecovery.AI;

/// <summary>프레임 보간(placeholder). Phase 5 에서 AI 모델 연결. (요구사항 8 Level 4)</summary>
public sealed class FrameInterpolation : IFrameInterpolation
{
    private readonly ILogger<FrameInterpolation> _logger;

    public FrameInterpolation(ILogger<FrameInterpolation> logger) => _logger = logger;

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> InterpolateAsync(
        string beforeFrameImagePath,
        string afterFrameImagePath,
        int count,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Frame interpolation requested ({Count} frames)", count);
        throw new NotImplementedException(
            "FrameInterpolation.InterpolateAsync: Phase 5 에서 구현한다.");
    }
}
