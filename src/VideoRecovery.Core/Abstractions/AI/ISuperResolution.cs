namespace VideoRecovery.Core.Abstractions.AI;

/// <summary>
/// 초해상(Super Resolution). 영상 품질을 최대한 복원한다. (요구사항 8 Level 4)
/// </summary>
public interface ISuperResolution
{
    /// <summary>
    /// 입력 이미지를 <paramref name="scale"/> 배로 업스케일한 결과 경로를 반환한다.
    /// </summary>
    Task<string> UpscaleAsync(
        string inputImagePath,
        int scale,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}
