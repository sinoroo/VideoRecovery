using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 프레임/구조 손상 탐지 및 유형 분류. Corruption Detection Agent. (요구사항 7, 16)
/// TODO(Phase 1): HealthReport + 프레임 이상 결과를 CorruptionType 으로 매핑.
/// </summary>
public sealed class CorruptionDetector : ICorruptionDetector
{
    private readonly ILogger<CorruptionDetector> _logger;

    public CorruptionDetector(ILogger<CorruptionDetector> logger) => _logger = logger;

    /// <inheritdoc />
    public Task<CorruptionReport> DetectAsync(
        VideoFileInfo file,
        HealthReport health,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Corruption detection requested for {FilePath}", file.FilePath);
        throw new NotImplementedException(
            "CorruptionDetector.DetectAsync: 손상 유형 분류를 Phase 1 에서 구현한다.");
    }
}
