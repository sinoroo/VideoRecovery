namespace VideoRecovery.Core.Abstractions.AI;

/// <summary>AI 복원 요청의 공통 컨텍스트.</summary>
public sealed class AiRecoveryRequest
{
    /// <summary>입력 영상/프레임 경로.</summary>
    public required string InputPath { get; init; }

    /// <summary>결과물 출력 경로.</summary>
    public required string OutputPath { get; init; }

    /// <summary>대상 구간 시작 프레임(선택).</summary>
    public long? StartFrame { get; init; }

    /// <summary>대상 구간 끝 프레임(선택).</summary>
    public long? EndFrame { get; init; }
}

/// <summary>
/// AI 추론 엔진 추상화. 실제 모델(ONNX/CUDA/TensorRT)은 후속 단계에서 연결한다.
/// AI 복구는 기존 데이터 복구와 분리된 후속 복구 단계이다.
/// </summary>
public interface IAIInferenceEngine
{
    /// <summary>현재 환경에서 AI 추론이 가능한지 여부(모델/GPU 준비 상태).</summary>
    bool IsAvailable { get; }

    /// <summary>엔진/모델을 초기화한다.</summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
