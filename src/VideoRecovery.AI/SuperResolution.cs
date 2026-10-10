using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.AI;

namespace VideoRecovery.AI;

/// <summary>초해상(placeholder). Phase 5 에서 AI 모델 연결. (요구사항 8 Level 4)</summary>
public sealed class SuperResolution : ISuperResolution
{
    private readonly ILogger<SuperResolution> _logger;

    public SuperResolution(ILogger<SuperResolution> logger) => _logger = logger;

    /// <inheritdoc />
    public Task<string> UpscaleAsync(
        string inputImagePath,
        int scale,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Super resolution requested (x{Scale})", scale);
        throw new NotImplementedException(
            "SuperResolution.UpscaleAsync: Phase 5 에서 구현한다.");
    }
}
