using VideoRecovery.Core.Models;

namespace VideoRecovery.App.Services;

/// <summary>단일 영상에 대한 종합 진단 결과(스캔 → 분석 → 분류 → 매칭 → 계획).</summary>
public sealed class DiagnosisResult
{
    /// <summary>스캔된 파일 기본 정보.</summary>
    public required VideoFileInfo File { get; init; }

    /// <summary>건강 리포트.</summary>
    public HealthReport? Health { get; init; }

    /// <summary>손상 분류.</summary>
    public CorruptionReport? Corruption { get; init; }

    /// <summary>참조 후보.</summary>
    public IReadOnlyList<ReferenceCandidate> Candidates { get; init; } = Array.Empty<ReferenceCandidate>();

    /// <summary>복구 계획.</summary>
    public RecoveryPlan? Plan { get; init; }

    /// <summary>진단 중 발생한 오류(있는 경우).</summary>
    public string? Error { get; init; }
}
