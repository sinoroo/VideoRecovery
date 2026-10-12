namespace VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 프레임 특징 추출기. 프레임 바이트에서 비교용 특징(pHash/히스토그램 등)을 만든다.
/// </summary>
public interface IFeatureMatcher
{
    /// <summary>프레임 이미지 바이트에서 perceptual hash(64-bit)를 계산한다.</summary>
    ulong ComputePerceptualHash(byte[] frameImage);

    /// <summary>두 perceptual hash의 유사도(0.0 ~ 1.0)를 반환한다.</summary>
    double HashSimilarity(ulong a, ulong b);
}
