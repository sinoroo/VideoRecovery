using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;
using VideoRecovery.Recovery;
using Xunit;

namespace VideoRecovery.UnitTests;

public sealed class RecoveryValidatorTests
{
    private readonly Mock<IHealthAnalyzer> _healthAnalyzer = new();

    private RecoveryValidator CreateSut()
        => new(_healthAnalyzer.Object, NullLogger<RecoveryValidator>.Instance);

    [Fact]
    public async Task ValidateAsync_WhenRecoveryFailed_SkipsReanalysis()
    {
        // Arrange
        var sut = CreateSut();
        var recovery = TestData.RecoveryFailure();

        // Act
        var result = await sut.ValidateAsync(recovery, TestData.Health());

        // Assert
        result.Playable.Should().BeFalse();
        result.Improved.Should().BeFalse();
        _healthAnalyzer.Verify(a => a.AnalyzeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidateAsync_WhenErrorsReduced_ReportsImprovement()
    {
        // Arrange
        var before = TestData.Health(errorFrames: 10, severity: HealthSeverity.Severe);
        var after = TestData.Health(errorFrames: 0, severity: HealthSeverity.Healthy);
        _healthAnalyzer.Setup(a => a.AnalyzeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(after);
        var sut = CreateSut();

        // Act
        var result = await sut.ValidateAsync(TestData.RecoverySuccess(), before);

        // Assert
        result.Playable.Should().BeTrue();
        result.FullyDecodable.Should().BeTrue();
        result.Improved.Should().BeTrue();
        result.ImprovementScore.Should().Be(1.0);
    }

    [Fact]
    public async Task ValidateAsync_WhenErrorsPartiallyReduced_ReportsPartialImprovement()
    {
        // Arrange
        var before = TestData.Health(errorFrames: 10, severity: HealthSeverity.Severe);
        var after = TestData.Health(errorFrames: 4, severity: HealthSeverity.Moderate);
        _healthAnalyzer.Setup(a => a.AnalyzeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(after);
        var sut = CreateSut();

        // Act
        var result = await sut.ValidateAsync(TestData.RecoverySuccess(), before);

        // Assert — (10 - 4) / 10 = 0.6
        result.ImprovementScore.Should().BeApproximately(0.6, 0.0001);
        result.FullyDecodable.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateAsync_WhenRecoveredHasNoVideoStream_NotPlayable()
    {
        // Arrange
        var after = TestData.Health(errorFrames: 0, withVideo: false);
        _healthAnalyzer.Setup(a => a.AnalyzeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(after);
        var sut = CreateSut();

        // Act
        var result = await sut.ValidateAsync(TestData.RecoverySuccess(), TestData.Health(errorFrames: 5));

        // Assert
        result.Playable.Should().BeFalse();
    }
}
