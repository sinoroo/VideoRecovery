using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;
using VideoRecovery.Recovery;
using Xunit;

namespace VideoRecovery.UnitTests;

public sealed class RecoveryExecutorTests
{
    private readonly Mock<IWorkspaceManager> _workspace = new();
    private readonly Mock<IRecoveryValidator> _validator = new();

    public RecoveryExecutorTests()
    {
        _workspace.Setup(w => w.CreateSession(It.IsAny<string>())).Returns(@"C:\ws\session");
        _workspace.Setup(w => w.GetArtifactPath(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((s, f) => System.IO.Path.Combine(s, f));
    }

    private RecoveryExecutor CreateSut(params IRecoveryStrategy[] strategies)
        => new(strategies, _workspace.Object, _validator.Object, NullLogger<RecoveryExecutor>.Instance);

    private static Mock<IRecoveryStrategy> Strategy(RecoveryLevel level, RecoveryResult result)
    {
        var mock = new Mock<IRecoveryStrategy>();
        mock.SetupGet(s => s.Level).Returns(level);
        mock.Setup(s => s.RecoverAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CorruptionReport>(),
                It.IsAny<ReferenceCandidate?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        return mock;
    }

    private static RecoveryPlan Plan(params RecoveryLevel[] levels)
        => new()
        {
            FullPath = @"C:\videos\clip.mp4",
            Steps = levels.Select(l => new RecoveryStep { Level = l, EstimatedConfidence = 0.7 }).ToList()
        };

    [Fact]
    public async Task ExecuteAsync_NoSteps_ReturnsNotRecoveredWithoutCreatingSession()
    {
        // Arrange
        var sut = CreateSut();
        var plan = Plan();

        // Act
        var outcome = await sut.ExecuteAsync(
            @"C:\videos\clip.mp4", TestData.Health(),
            TestData.Corruption(types: CorruptionType.None, severity: HealthSeverity.Healthy), plan);

        // Assert
        outcome.Succeeded.Should().BeFalse();
        _workspace.Verify(w => w.CreateSession(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ContainerSucceedsAndFullyPlayable_StopsAfterFirstStep()
    {
        // Arrange
        var container = Strategy(RecoveryLevel.ContainerRecovery, TestData.RecoverySuccess(level: RecoveryLevel.ContainerRecovery));
        var frame = Strategy(RecoveryLevel.FrameRecovery, TestData.RecoverySuccess(level: RecoveryLevel.FrameRecovery));
        _validator.Setup(v => v.ValidateAsync(It.IsAny<RecoveryResult>(), It.IsAny<HealthReport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Validation(playable: true, fullyDecodable: true));
        var sut = CreateSut(container.Object, frame.Object);

        // Act
        var outcome = await sut.ExecuteAsync(
            @"C:\videos\clip.mp4", TestData.Health(), TestData.Corruption(),
            Plan(RecoveryLevel.ContainerRecovery, RecoveryLevel.FrameRecovery));

        // Assert
        outcome.Succeeded.Should().BeTrue();
        outcome.Recovery!.AppliedLevel.Should().Be(RecoveryLevel.ContainerRecovery);
        frame.Verify(s => s.RecoverAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CorruptionReport>(),
            It.IsAny<ReferenceCandidate?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ContainerFails_FallsBackToFrameRecovery()
    {
        // Arrange
        var container = Strategy(RecoveryLevel.ContainerRecovery, TestData.RecoveryFailure(level: RecoveryLevel.ContainerRecovery));
        var frame = Strategy(RecoveryLevel.FrameRecovery, TestData.RecoverySuccess(level: RecoveryLevel.FrameRecovery));
        _validator.Setup(v => v.ValidateAsync(It.IsAny<RecoveryResult>(), It.IsAny<HealthReport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Validation(playable: true, fullyDecodable: true));
        var sut = CreateSut(container.Object, frame.Object);

        // Act
        var outcome = await sut.ExecuteAsync(
            @"C:\videos\clip.mp4", TestData.Health(), TestData.Corruption(),
            Plan(RecoveryLevel.ContainerRecovery, RecoveryLevel.FrameRecovery));

        // Assert
        outcome.Succeeded.Should().BeTrue();
        outcome.Recovery!.AppliedLevel.Should().Be(RecoveryLevel.FrameRecovery);
    }

    [Fact]
    public async Task ExecuteAsync_NotImplementedStrategy_IsSkipped()
    {
        // Arrange
        var crossVideo = new Mock<IRecoveryStrategy>();
        crossVideo.SetupGet(s => s.Level).Returns(RecoveryLevel.CrossVideoRecovery);
        crossVideo.Setup(s => s.RecoverAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CorruptionReport>(),
                It.IsAny<ReferenceCandidate?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotImplementedException("Phase 4"));
        var container = Strategy(RecoveryLevel.ContainerRecovery, TestData.RecoverySuccess());
        _validator.Setup(v => v.ValidateAsync(It.IsAny<RecoveryResult>(), It.IsAny<HealthReport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Validation(playable: true, fullyDecodable: true));
        var sut = CreateSut(crossVideo.Object, container.Object);

        // Act — plan lists cross-video first (unimplemented), then container.
        var outcome = await sut.ExecuteAsync(
            @"C:\videos\clip.mp4", TestData.Health(), TestData.Corruption(),
            Plan(RecoveryLevel.CrossVideoRecovery, RecoveryLevel.ContainerRecovery));

        // Assert
        outcome.Succeeded.Should().BeTrue();
        outcome.Recovery!.AppliedLevel.Should().Be(RecoveryLevel.ContainerRecovery);
        outcome.Attempts.Should().Contain(a => a.Contains("미구현"));
    }

    [Fact]
    public async Task ExecuteAsync_AllStrategiesFail_ReturnsFailureOutcome()
    {
        // Arrange
        var container = Strategy(RecoveryLevel.ContainerRecovery, TestData.RecoveryFailure(level: RecoveryLevel.ContainerRecovery));
        var frame = Strategy(RecoveryLevel.FrameRecovery, TestData.RecoveryFailure(level: RecoveryLevel.FrameRecovery));
        var sut = CreateSut(container.Object, frame.Object);

        // Act
        var outcome = await sut.ExecuteAsync(
            @"C:\videos\clip.mp4", TestData.Health(), TestData.Corruption(),
            Plan(RecoveryLevel.ContainerRecovery, RecoveryLevel.FrameRecovery));

        // Assert
        outcome.Succeeded.Should().BeFalse();
        outcome.Summary.Should().Contain("실패");
        _validator.Verify(v => v.ValidateAsync(It.IsAny<RecoveryResult>(), It.IsAny<HealthReport>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_SucceedsButNotFullyDecodable_TriesNextStepForBetterResult()
    {
        // Arrange
        var container = Strategy(RecoveryLevel.ContainerRecovery, TestData.RecoverySuccess(level: RecoveryLevel.ContainerRecovery));
        var frame = Strategy(RecoveryLevel.FrameRecovery, TestData.RecoverySuccess(level: RecoveryLevel.FrameRecovery));
        _validator.SetupSequence(v => v.ValidateAsync(It.IsAny<RecoveryResult>(), It.IsAny<HealthReport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Validation(playable: true, fullyDecodable: false, improvement: 0.5))
            .ReturnsAsync(TestData.Validation(playable: true, fullyDecodable: true, improvement: 1.0));
        var sut = CreateSut(container.Object, frame.Object);

        // Act
        var outcome = await sut.ExecuteAsync(
            @"C:\videos\clip.mp4", TestData.Health(errorFrames: 10, severity: HealthSeverity.Moderate),
            TestData.Corruption(), Plan(RecoveryLevel.ContainerRecovery, RecoveryLevel.FrameRecovery));

        // Assert — 컨테이너는 성공했지만 완전 디코딩이 아니므로 프레임 복구까지 진행하여 더 나은 결과 채택.
        outcome.Succeeded.Should().BeTrue();
        outcome.Recovery!.AppliedLevel.Should().Be(RecoveryLevel.FrameRecovery);
        frame.Verify(s => s.RecoverAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CorruptionReport>(),
            It.IsAny<ReferenceCandidate?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
