using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>복구 실행 결과. 원본은 보존되고 결과물은 작업 영역에 생성된다.</summary>
public sealed class RecoveryResult
{
    /// <summary>원본 파일 경로(변경되지 않음).</summary>
    public required string OriginalPath { get; init; }

    /// <summary>복구 결과물 경로(작업 영역).</summary>
    public string? RecoveredPath { get; init; }

    /// <summary>적용된 복구 레벨.</summary>
    public RecoveryLevel AppliedLevel { get; init; }

    /// <summary>성공 여부.</summary>
    public bool Succeeded { get; init; }

    /// <summary>결과 신뢰도(0.0 ~ 1.0).</summary>
    public double Confidence { get; init; }

    /// <summary>수행 로그/메시지.</summary>
    public IReadOnlyList<string> Log { get; init; } = Array.Empty<string>();

    /// <summary>실패 사유(있는 경우).</summary>
    public string? Error { get; init; }
}
