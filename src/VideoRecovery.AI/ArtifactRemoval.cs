using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.AI;

namespace VideoRecovery.AI;

/// <summary>아티팩트 제거(placeholder). Phase 5 에서 AI 모델 연결. (요구사항 8 Level 4)</summary>
public sealed class ArtifactRemoval : IArtifactRemoval
{
    private readonly ILogger<ArtifactRemoval> _logger;

    public ArtifactRemoval(ILogger<ArtifactRemoval> logger) => _logger = logger;

    /// <inheritdoc />
    public Task<string> RemoveAsync(
        string inputImagePath,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Artifact removal requested for {Input}", inputImagePath);
        throw new NotImplementedException(
            "ArtifactRemoval.RemoveAsync: Phase 5 에서 구현한다.");
    }
}
