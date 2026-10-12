using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.UnitTests;

/// <summary>테스트용 모델 빌더 모음. 각 테스트가 필요한 부분만 덮어쓸 수 있게 기본값을 제공한다.</summary>
internal static class TestData
{
    public static VideoFileInfo File(string path = @"C:\videos\clip.mp4", long size = 1_000_000)
        => new()
        {
            FullPath = path,
            FileName = System.IO.Path.GetFileName(path),
            SizeBytes = size,
            Container = "mp4"
        };

    public static VideoMetadata Metadata(string path = @"C:\videos\clip.mp4", bool withVideo = true)
        => new()
        {
            FullPath = path,
            FormatName = "mov,mp4,m4a",
            Duration = TimeSpan.FromSeconds(60),
            Streams = withVideo
                ? new[] { new StreamInfo { Index = 0, Kind = StreamKind.Video, Codec = "h264", Width = 1920, Height = 1080, Fps = 30 } }
                : Array.Empty<StreamInfo>()
        };

    public static HealthReport Health(
        string path = @"C:\videos\clip.mp4",
        long errorFrames = 0,
        HealthSeverity severity = HealthSeverity.Healthy,
        bool withVideo = true)
    {
        var segments = errorFrames > 0
            ? new[] { new DecodeSegment { StartFrame = 100, EndFrame = 100 + errorFrames - 1, IsError = true } }
            : Array.Empty<DecodeSegment>();

        return new HealthReport
        {
            FullPath = path,
            Metadata = Metadata(path, withVideo),
            DecodeSegments = segments,
            Severity = severity,
            Summary = severity == HealthSeverity.Healthy ? "정상" : "손상 감지"
        };
    }

    public static CorruptionReport Corruption(
        string path = @"C:\videos\clip.mp4",
        CorruptionType types = CorruptionType.ContainerCorruption,
        HealthSeverity severity = HealthSeverity.Moderate)
        => new()
        {
            FullPath = path,
            Types = types,
            Severity = severity
        };

    public static ReferenceCandidate Reference(string path = @"C:\videos\reference.mp4", double confidence = 0.9)
        => new()
        {
            ReferencePath = path,
            Confidence = confidence,
            MetadataSimilarity = confidence,
            VisualSimilarity = confidence,
            Reason = "test reference"
        };

    public static RecoveryResult RecoverySuccess(
        string original = @"C:\videos\clip.mp4",
        string recovered = @"C:\ws\clip_recovered.mp4",
        RecoveryLevel level = RecoveryLevel.ContainerRecovery,
        double confidence = 0.8)
        => new()
        {
            OriginalPath = original,
            RecoveredPath = recovered,
            AppliedLevel = level,
            Succeeded = true,
            Confidence = confidence
        };

    public static RecoveryResult RecoveryFailure(
        string original = @"C:\videos\clip.mp4",
        RecoveryLevel level = RecoveryLevel.ContainerRecovery)
        => new()
        {
            OriginalPath = original,
            AppliedLevel = level,
            Succeeded = false,
            Confidence = 0,
            Error = "실패"
        };

    public static ValidationResult Validation(bool playable, bool fullyDecodable, double improvement = 1.0)
        => new()
        {
            RecoveredPath = @"C:\ws\clip_recovered.mp4",
            Playable = playable,
            FullyDecodable = fullyDecodable,
            Improved = improvement > 0,
            ImprovementScore = improvement
        };
}
