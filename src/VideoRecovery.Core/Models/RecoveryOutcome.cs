namespace VideoRecovery.Core.Models;

/// <summary>
/// 복구 실행 오케스트레이션의 최종 산출물. 적용된 복구 결과와 검증 결과를 함께 담는다.
/// 원본은 변경되지 않으며 결과물은 작업 영역 세션에 생성된다.
/// </summary>
public sealed class RecoveryOutcome
{
    /// <summary>원본 파일 경로(변경되지 않음).</summary>
    public required string OriginalPath { get; init; }

    /// <summary>작업 세션 경로.</summary>
    public string SessionPath { get; init; } = string.Empty;

    /// <summary>최종 채택된 복구 결과(없으면 null).</summary>
    public RecoveryResult? Recovery { get; init; }

    /// <summary>복구 결과 검증(없으면 null).</summary>
    public ValidationResult? Validation { get; init; }

    /// <summary>시도한 복구 단계별 로그.</summary>
    public IReadOnlyList<string> Attempts { get; init; } = Array.Empty<string>();

    /// <summary>복구 성공 여부.</summary>
    public bool Succeeded => Recovery is { Succeeded: true };

    /// <summary>복구 결과물 경로(성공 시).</summary>
    public string? RecoveredPath => Recovery?.RecoveredPath;

    /// <summary>사용자 표시용 요약.</summary>
    public string Summary { get; init; } = string.Empty;
}
