using CommunityToolkit.Mvvm.ComponentModel;
using VideoRecovery.App.Services;
using VideoRecovery.Core.Enums;

namespace VideoRecovery.App.ViewModels;

/// <summary>
/// 진단 결과 그리드의 단일 행. 파일명/심각도/손상유형/복구가능성/추천방법/참조를
/// 표시용 문자열로 노출한다.
/// </summary>
public sealed class DiagnosisRowViewModel
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

    /// <summary>손상 여부(행 하이라이트에 사용).</summary>
    public bool IsDamaged => Result.Corruption is { IsHealthy: false };

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
