using System.Numerics;

namespace VideoRecovery.CrossMatch;

using VideoRecovery.Core.Abstractions.CrossMatch;

/// <summary>
/// 프레임 특징 비교기. 간단한 average-hash 기반 perceptual hash와 해밍 거리
/// 유사도를 제공한다(외부 OpenCV 없이 동작). 더 정밀한 SSIM/feature 비교는
/// OpenCV 연동(후속 단계)에서 확장한다.
/// </summary>
public sealed class FeatureMatcher : IFeatureMatcher
{
    public ulong ComputePerceptualHash(byte[] frameImage)
    {
        if (frameImage is null || frameImage.Length == 0)
        {
            return 0;
        }

        // 바이트 스트림을 64개 버킷으로 요약 → 평균 대비 상/하로 비트를 세운다(average hash 유사).
        Span<long> buckets = stackalloc long[64];
        Span<long> counts = stackalloc long[64];

        for (var i = 0; i < frameImage.Length; i++)
        {
            var bucket = (int)((long)i * 64 / frameImage.Length);
            buckets[bucket] += frameImage[i];
            counts[bucket]++;
        }

        Span<double> averages = stackalloc double[64];
        double total = 0;
        for (var i = 0; i < 64; i++)
        {
            averages[i] = counts[i] > 0 ? (double)buckets[i] / counts[i] : 0;
            total += averages[i];
        }

        var mean = total / 64.0;

        ulong hash = 0;
        for (var i = 0; i < 64; i++)
        {
            if (averages[i] >= mean)
            {
                hash |= 1UL << i;
            }
        }

        return hash;
    }

    public double HashSimilarity(ulong a, ulong b)
    {
        var distance = BitOperations.PopCount(a ^ b);
        return 1.0 - (distance / 64.0);
    }
}
