namespace VideoRecovery.Core.Abstractions.AI;

/// <summary>
/// 아티팩트 제거(디노이즈/디블러/블록 제거). (요구사항 8 Level 4)
/// </summary>
public interface IArtifactRemoval
{
    /// <summary>
    /// 입력 이미지에서 artifact 를 제거한 결과 경로를 반환한다.
    /// </summary>
    Task<string> RemoveAsync(
        string inputImagePath,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}
