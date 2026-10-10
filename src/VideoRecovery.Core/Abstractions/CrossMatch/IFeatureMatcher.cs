namespace VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 프레임 특징 추출/비교. (요구사항 6 - 2단계 Feature comparison)
/// pHash/Histogram/Feature 등 저수준 특징은 Infrastructure(OpenCV) 구현에 위임한다.
/// </summary>
public interface IFeatureMatcher
{
    /// <summary>
    /// 두 프레임 이미지 경로의 특징 유사도를 0.0~1.0 으로 반환한다.
    /// </summary>
    Task<double> CompareAsync(string frameImagePathA, string frameImagePathB, CancellationToken cancellationToken = default);
}
