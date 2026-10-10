using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.AI;

namespace VideoRecovery.AI;

/// <summary>
/// AI 추론 엔진 기본 구현(placeholder). 실제 모델/런타임(ONNX/CUDA/TensorRT)은 Phase 5 에서 연결.
/// AI 복구는 기존 데이터 복구와 분리된 후속 단계이다. (요구사항 12, 17-7)
/// </summary>
public sealed class NullAIInferenceEngine : IAIInferenceEngine
{
    private readonly ILogger<NullAIInferenceEngine> _logger;

    public NullAIInferenceEngine(ILogger<NullAIInferenceEngine> logger) => _logger = logger;

    /// <inheritdoc />
    /// <remarks>Phase 5 전까지는 AI 런타임이 준비되지 않았음을 의미한다.</remarks>
    public bool IsAvailable => false;

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> RunAsync(
        IReadOnlyList<string> inputImagePaths,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("AI inference engine is not available yet (Phase 5).");
        throw new NotImplementedException(
            "NullAIInferenceEngine.RunAsync: AI 모델 연결은 Phase 5 에서 구현한다.");
    }
}
