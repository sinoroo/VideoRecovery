using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;
using VideoRecovery.Recovery;
using Xunit;

namespace VideoRecovery.UnitTests;

public sealed class RecoveryPlannerTests
{
    private static RecoveryPlanner CreateSut()
        => new(NullLogger<RecoveryPlanner>.Instance);

    [Fact]
    public void CreatePlan_Healthy_ReturnsNoStepsAndFullProbability()
    {
        // Arrange
        var sut = CreateSut();
        var corruption = new CorruptionReport
        {
            FullPath = @"C:\v\a.mp4",
            Types = CorruptionType.None,
            Severity = HealthSeverity.Healthy
        };

        // Act
        var plan = sut.CreatePlan(corruption, Array.Empty<ReferenceCandidate>());

        // Assert
        plan.Steps.Should().BeEmpty();
        plan.RecoveryProbability.Should().Be(1.0);
        plan.Summary.Should().Contain("손상 없음");
    }

    [Fact]
    public void CreatePlan_ContainerCorruption_AddsContainerRecoveryFirst()
    {
        // Arrange
        var sut = CreateSut();
        var corruption = TestData.Corruption(types: CorruptionType.ContainerCorruption, severity: HealthSeverity.Moderate);

        // Act
        var plan = sut.CreatePlan(corruption, Array.Empty<ReferenceCandidate>());

        // Assert
        plan.Steps.Should().ContainSingle();
        plan.Steps[0].Level.Should().Be(RecoveryLevel.ContainerRecovery);
        plan.RecoveryProbability.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CreatePlan_FrameCorruption_AddsFrameRecovery()
    {
        // Arrange
        var sut = CreateSut();
        var corruption = TestData.Corruption(types: CorruptionType.DecodeError, severity: HealthSeverity.Moderate);

        // Act
        var plan = sut.CreatePlan(corruption, Array.Empty<ReferenceCandidate>());

        // Assert
        plan.Steps.Should().Contain(s => s.Level == RecoveryLevel.FrameRecovery);
    }

    [Fact]
    public void CreatePlan_SevereWithReference_AddsCrossVideoStepWithRecommendedReference()
    {
        // Arrange
        var sut = CreateSut();
        var corruption = TestData.Corruption(types: CorruptionType.CorruptedFrame, severity: HealthSeverity.Severe);
        var candidate = TestData.Reference(confidence: 0.85);

        // Act
        var plan = sut.CreatePlan(corruption, new[] { candidate });

        // Assert
        plan.RecommendedReference.Should().Be(candidate);
        plan.Steps.Should().Contain(s => s.Level == RecoveryLevel.CrossVideoRecovery);
    }

    [Fact]
    public void CreatePlan_StepsOrderedBySafestLevelFirst()
    {
        // Arrange
        var sut = CreateSut();
        var corruption = TestData.Corruption(
            types: CorruptionType.ContainerCorruption | CorruptionType.DecodeError,
            severity: HealthSeverity.Severe);

        // Act
        var plan = sut.CreatePlan(corruption, Array.Empty<ReferenceCandidate>());

        // Assert — 컨테이너(Level 1)가 프레임(Level 2)보다 먼저 온다.
        var levels = plan.Steps.Select(s => (int)s.Level).ToList();
        levels.Should().BeInAscendingOrder();
        plan.Steps[0].Level.Should().Be(RecoveryLevel.ContainerRecovery);
    }

    [Fact]
    public void CreatePlan_Unrecoverable_YieldsLowProbability()
    {
        // Arrange
        var sut = CreateSut();
        var corruption = TestData.Corruption(types: CorruptionType.DecodeError, severity: HealthSeverity.Unrecoverable);

        // Act
        var plan = sut.CreatePlan(corruption, Array.Empty<ReferenceCandidate>());

        // Assert
        plan.RecoveryProbability.Should().BeLessThan(0.5);
    }
}
