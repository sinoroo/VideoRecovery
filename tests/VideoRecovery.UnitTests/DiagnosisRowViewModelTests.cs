using FluentAssertions;
using VideoRecovery.App.Services;
using VideoRecovery.App.ViewModels;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;
using Xunit;

namespace VideoRecovery.UnitTests;

public sealed class DiagnosisRowViewModelTests
{
    [Fact]
    public void Ctor_HealthyResult_NotDamagedAndCannotRecover()
    {
        // Arrange
        var result = new DiagnosisResult
        {
            File = TestData.File(@"C:\v\ok.mp4"),
            Health = TestData.Health(@"C:\v\ok.mp4"),
            Corruption = TestData.Corruption(@"C:\v\ok.mp4", CorruptionType.None, HealthSeverity.Healthy),
            Plan = new RecoveryPlan { FullPath = @"C:\v\ok.mp4", Steps = Array.Empty<RecoveryStep>() }
        };

        // Act
        var row = new DiagnosisRowViewModel(result);

        // Assert
        row.IsDamaged.Should().BeFalse();
        row.CanRecover.Should().BeFalse();
        row.CorruptionText.Should().Be("정상");
        row.SeverityText.Should().Be("정상");
    }

    [Fact]
    public void Ctor_DamagedResult_IsDamagedAndCanRecover()
    {
        // Arrange
        var result = new DiagnosisResult
        {
            File = TestData.File(@"C:\v\bad.mov", size: 2_500_000),
            Health = TestData.Health(@"C:\v\bad.mov", errorFrames: 5, severity: HealthSeverity.Severe),
            Corruption = TestData.Corruption(@"C:\v\bad.mov", CorruptionType.DecodeError, HealthSeverity.Severe),
            Plan = new RecoveryPlan
            {
                FullPath = @"C:\v\bad.mov",
                Steps = new[] { new RecoveryStep { Level = RecoveryLevel.FrameRecovery } },
                RecoveryProbability = 0.57
            }
        };

        // Act
        var row = new DiagnosisRowViewModel(result);

        // Assert
        row.IsDamaged.Should().BeTrue();
        row.CanRecover.Should().BeTrue();
        row.SeverityText.Should().Be("심각");
        row.RecommendedMethod.Should().Be("FrameRecovery");
        row.RecoveryProbabilityText.Should().Be(0.57.ToString("P0"));
    }

    [Fact]
    public void RecoveryStatus_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var row = new DiagnosisRowViewModel(new DiagnosisResult
        {
            File = TestData.File(),
            Corruption = TestData.Corruption()
        });
        using var monitor = row.Monitor();

        // Act
        row.RecoveryStatus = "성공";

        // Assert
        monitor.Should().RaisePropertyChangeFor(r => r.RecoveryStatus);
    }

    [Theory]
    [InlineData(512L, "512 B")]
    [InlineData(1024L, "1 KB")]
    [InlineData(1_572_864L, "1.5 MB")]
    public void Ctor_FormatsSizeText(long bytes, string expected)
    {
        // Arrange
        var result = new DiagnosisResult { File = TestData.File(size: bytes), Corruption = TestData.Corruption() };

        // Act
        var row = new DiagnosisRowViewModel(result);

        // Assert
        row.SizeText.Should().Be(expected);
    }
}
