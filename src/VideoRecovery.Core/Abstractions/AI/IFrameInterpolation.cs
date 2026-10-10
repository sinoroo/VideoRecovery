namespace VideoRecovery.Core.Abstractions.AI;

/// <summary>
/// 프레임 보간. 누락 프레임을 주변 프레임으로부터 생성한다. (요구사항 8 Level 4)
/// </summary>
public interface IFrameInterpolation
{
    /// <summary>
    /// 두 프레임 사이에 <paramref name="count"/> 개의 중간 프레임을 생성한다.
    /// </summary>
    Task<IReadOnlyList<string>> InterpolateAsync(
        string beforeFrameImagePath,
        string afterFrameImagePath,
        int count,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}
