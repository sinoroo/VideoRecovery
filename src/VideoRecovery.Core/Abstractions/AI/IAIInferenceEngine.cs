namespace VideoRecovery.Core.Abstractions.AI;

/// <summary>
/// AI 추론 엔진 추상화. 초기에는 계약만 정의하고 실제 모델(ONNX/CUDA/TensorRT)은
/// 후속 단계에서 연결한다. AI 복구는 기존 데이터 복구와 분리한다. (요구사항 12, 17-7)
/// </summary>
public interface IAIInferenceEngine
{
    /// <summary>엔진 사용 가능 여부(모델/런타임/GPU 준비 상태).</summary>
    bool IsAvailable { get; }

    /// <summary>
    /// 입력 이미지들에 대해 지정한 추론을 수행하고 결과 이미지 경로를 반환한다.
    /// </summary>
    Task<IReadOnlyList<string>> RunAsync(
        IReadOnlyList<string> inputImagePaths,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}
