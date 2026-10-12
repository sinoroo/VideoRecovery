namespace VideoRecovery.App.Services;

/// <summary>진단 파이프라인 진행 상황 보고 모델.</summary>
public sealed class DiagnosisProgress
{
    /// <summary>현재 처리 중인 파일 경로.</summary>
    public string CurrentFile { get; init; } = string.Empty;

    /// <summary>처리 완료된 파일 수.</summary>
    public int Completed { get; init; }

    /// <summary>전체 파일 수.</summary>
    public int Total { get; init; }

    /// <summary>현재 단계 설명.</summary>
    public string Stage { get; init; } = string.Empty;
}

/// <summary>
/// MVP 진단 파이프라인. 폴더 스캔 → 메타데이터/디코드 분석 → 손상 분류 →
/// 참조 후보 매칭 → 복구 계획까지를 하나의 흐름으로 수행한다. 원본은 변경하지 않는다.
/// </summary>
public interface IDiagnosisPipeline
{
    /// <summary>지정 폴더를 진단한다.</summary>
    Task<IReadOnlyList<DiagnosisResult>> RunAsync(
        string rootDirectory,
        bool recursive = true,
        IProgress<DiagnosisProgress>? progress = null,
        CancellationToken cancellationToken = default);
}
