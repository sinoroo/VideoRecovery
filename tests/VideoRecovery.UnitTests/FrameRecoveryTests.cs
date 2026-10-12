using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Enums;
using VideoRecovery.Recovery;
using Xunit;

namespace VideoRecovery.UnitTests;

public sealed class FrameRecoveryTests
{
    private readonly Mock<IFFmpegService> _ffmpeg = new();
    private readonly Mock<IWorkspaceManager> _workspace = new();

    public FrameRecoveryTests()
        => _workspace.Setup(w => w.GetArtifactPath(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((s, f) => System.IO.Path.Combine(s, f));

    private FrameRecovery CreateSut()
        => new(_ffmpeg.Object, _workspace.Object, NullLogger<FrameRecovery>.Instance);

    [Theory]
    [InlineData(CorruptionType.DecodeError, true)]
    [InlineData(CorruptionType.MissingFrame, true)]
    [InlineData(CorruptionType.CorruptedFrame, true)]
    [InlineData(CorruptionType.ContainerCorruption, false)]
    [InlineData(CorruptionType.None, false)]
    public void CanHandle_ReturnsExpected(CorruptionType types, bool expected)
    {
        // Arrange
        var sut = CreateSut();

        // Act & Assert
        sut.CanHandle(types).Should().Be(expected);
    }

    [Fact]
    public async Task RecoverAsync_WhenReencodeSucceeds_ReturnsSuccessWithRecoveredPath()
    {
        // Arrange
        _ffmpeg.Setup(f => f.ReencodeDropCorruptAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var sut = CreateSut();

        // Act
        var result = await sut.RecoverAsync(
            @"C:\videos\clip.mp4", @"C:\ws\session", TestData.Corruption(types: CorruptionType.DecodeError), reference: null);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.AppliedLevel.Should().Be(RecoveryLevel.FrameRecovery);
        result.RecoveredPath.Should().EndWith("_frames_recovered.mp4");
        result.Confidence.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task RecoverAsync_WhenReencodeFails_ReturnsFailure()
    {
        // Arrange
        _ffmpeg.Setup(f => f.ReencodeDropCorruptAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var sut = CreateSut();

        // Act
        var result = await sut.RecoverAsync(
            @"C:\videos\clip.mp4", @"C:\ws\session", TestData.Corruption(types: CorruptionType.DecodeError), reference: null);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.RecoveredPath.Should().BeNull();
        result.Error.Should().NotBeNullOrEmpty();
    }
}
