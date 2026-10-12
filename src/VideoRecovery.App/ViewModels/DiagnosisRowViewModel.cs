using CommunityToolkit.Mvvm.ComponentModel;
using VideoRecovery.App.Services;
using VideoRecovery.Core.Enums;

namespace VideoRecovery.App.ViewModels;

/// <summary>
/// 진단 결과 그리드의 단일 행. 파일명/심각도/손상유형/복구가능성/추천방법/참조를
/// 표시용 문자열로 노출한다. 복구 상태는 실행 중 갱신되는 관찰 가능 속성이다.
/// </summary>
public sealed partial class DiagnosisRowViewModel : ObservableObject
{
    public DiagnosisRowViewModel(DiagnosisResult result)
    {
        Result = result;
        FileName = result.File.FileName;
        SizeText = FormatSize(result.File.SizeBytes);

        Severity = result.Corruption?.Severity ?? HealthSeverity.Healthy;
        SeverityText = ToKorean(Severity);

        CorruptionText = result.Corruption is { IsHealthy: false }
            ? result.Corruption.Types.ToString()
            : "정상";

        RecoveryProbabilityText = result.Plan is not null
            ? result.Plan.RecoveryProbability.ToString("P0")
            : "-";

        RecommendedMethod = result.Plan?.Steps.FirstOrDefault()?.Level.ToString() ?? "-";

        var reference = result.Plan?.RecommendedReference;
        ReferenceText = reference is not null
            ? System.IO.Path.GetFileName(reference.ReferencePath)
            : "-";

        Summary = result.Error ?? result.Health?.Summary ?? string.Empty;
    }

    /// <summary>원본 진단 결과.</summary>
    public DiagnosisResult Result { get; }

    public string FileName { get; }
    public string SizeText { get; }
    public HealthSeverity Severity { get; }
    public string SeverityText { get; }
    public string CorruptionText { get; }
    public string RecoveryProbabilityText { get; }
    public string RecommendedMethod { get; }
    public string ReferenceText { get; }
    public string Summary { get; }

    /// <summary>복구 상태(대기/복구 중/성공/실패/불필요).</summary>
    [ObservableProperty]
    private string _recoveryStatus = "-";

    /// <summary>복구 결과물 경로(성공 시).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRecovered))]
    private string _recoveredPath = string.Empty;

    /// <summary>복구 상세(검증/시도 로그 요약).</summary>
    [ObservableProperty]
    private string _recoveryDetail = string.Empty;

    /// <summary>손상 여부(행 하이라이트에 사용).</summary>
    public bool IsDamaged => Result.Corruption is { IsHealthy: false };

    /// <summary>복구 가능 여부(손상 + 복구 계획 존재).</summary>
    public bool CanRecover => IsDamaged && Result.Plan is { Steps.Count: > 0 };

    /// <summary>저장 가능한 복구 결과물이 생성되었는지 여부.</summary>
    public bool IsRecovered => !string.IsNullOrWhiteSpace(RecoveredPath);

    private static string ToKorean(HealthSeverity severity) => severity switch
    {
        HealthSeverity.Healthy => "정상",
        HealthSeverity.Minor => "경미",
        HealthSeverity.Moderate => "중간",
        HealthSeverity.Severe => "심각",
        HealthSeverity.Unrecoverable => "복구불가",
        _ => "-"
    };

    private static string FormatSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double size = bytes;
        var unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return $"{size:0.##} {units[unit]}";
    }
}
