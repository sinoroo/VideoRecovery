using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.CrossMatch;

namespace VideoRecovery.CrossMatch;

/// <summary>
/// 프레임 특징 비교(2단계). OpenCV 특징 추출/매칭에 위임. (요구사항 6)
/// TODO(Phase 3): 특징점(ORB/AKAZE) 추출 및 매칭 비율 산출.
/// </summary>
public sealed class FeatureMatcher : IFeatureMatcher
{
    private readonly ILogger<FeatureMatcher> _logger;

    public FeatureMatcher(ILogger<FeatureMatcher> logger) => _logger = logger;

    /// <inheritdoc />
    public Task<double> CompareAsync(string frameImagePathA, string frameImagePathB, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Feature compare {A} vs {B}", frameImagePathA, frameImagePathB);
        throw new NotImplementedException(
            "FeatureMatcher.CompareAsync: 특징 비교를 Phase 3 에서 구현한다.");
    }
}
