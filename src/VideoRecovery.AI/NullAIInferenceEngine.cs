using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.AI;

namespace VideoRecovery.AI;

/// <summary>
/// 실제 AI 모델이 연결되기 전의 기본 추론 엔진. 항상 <see cref="IsAvailable"/>=false를
/// 반환하여 상위 로직이 AI 복구를 건너뛰도록 한다. Phase 5에서 ONNX/CUDA/TensorRT
/// 백엔드로 교체한다.
/// </summary>
public sealed class NullAIInferenceEngine : IAIInferenceEngine
{
    private readonly ILogger<NullAIInferenceEngine> _logger;

    public NullAIInferenceEngine(ILogger<NullAIInferenceEngine> logger) => _logger = logger;

    public bool IsAvailable => false;

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("AI 추론 엔진 미연결(NullAIInferenceEngine) — Phase 5에서 모델 연결 예정");
        return Task.CompletedTask;
    }
}
