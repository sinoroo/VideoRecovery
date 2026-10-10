using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 복구 실행 결과. 원본은 수정하지 않고 작업 영역에 결과물을 생성한다. (요구사항 2.1, 8)
/// </summary>
public sealed class RecoveryResult
{
    /// <summary>원본 파일 경로(수정되지 않음).</summary>
    public required string SourceFilePath { get; init; }

    /// <summary>복구된 결과 파일 경로(작업 영역).</summary>
    public string? RecoveredFilePath { get; init; }

    /// <summary>적용된 복구 단계.</summary>
    public RecoveryLevel AppliedLevel { get; init; }

    /// <summary>복구 성공 여부.</summary>
    public bool Succeeded { get; init; }

    /// <summary>복구 신뢰도 점수 (0.0 ~ 1.0). (요구사항 17-4)</summary>
    public double Confidence { get; init; }

    /// <summary>실패 시 사유.</summary>
    public string? FailureReason { get; init; }

    /// <summary>복구 로그 파일 경로. (요구사항 17-2)</summary>
    public string? LogPath { get; init; }

    /// <summary>복구 수행 시각.</summary>
    public DateTimeOffset CompletedAt { get; init; } = DateTimeOffset.UtcNow;
}
