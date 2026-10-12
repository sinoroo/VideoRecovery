using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using VideoRecovery.App.Services;
using VideoRecovery.App.ViewModels;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;
using Xunit;

namespace VideoRecovery.UnitTests;

public sealed class MainViewModelTests
{
    private readonly Mock<IFolderPickerService> _folderPicker = new();
    private readonly Mock<IDiagnosisPipeline> _pipeline = new();
    private readonly Mock<IRecoveryExecutor> _executor = new();
    private readonly Mock<IFileSaveService> _fileSave = new();

    private MainViewModel CreateSut()
        => new(_folderPicker.Object, _pipeline.Object, _executor.Object, _fileSave.Object, NullLogger<MainViewModel>.Instance);

    private static DiagnosisResult DamagedResult(string path = @"C:\videos\bad.mp4")
        => new()
        {
            File = TestData.File(path),
            Health = TestData.Health(path, errorFrames: 5, severity: HealthSeverity.Moderate),
            Corruption = TestData.Corruption(path, CorruptionType.DecodeError, HealthSeverity.Moderate),
            Plan = new RecoveryPlan
            {
                FullPath = path,
                Steps = new[] { new RecoveryStep { Level = RecoveryLevel.FrameRecovery, EstimatedConfidence = 0.6 } },
                RecoveryProbability = 0.6
            }
        };

    private static DiagnosisResult HealthyResult(string path = @"C:\videos\ok.mp4")
        => new()
        {
            File = TestData.File(path),
            Health = TestData.Health(path),
            Corruption = TestData.Corruption(path, CorruptionType.None, HealthSeverity.Healthy),
            Plan = new RecoveryPlan { FullPath = path, Steps = Array.Empty<RecoveryStep>(), RecoveryProbability = 1.0 }
        };

    [Fact]
    public void CanAnalyze_NoFolder_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();

        // Act & Assert
        sut.CanAnalyze.Should().BeFalse();
    }

    [Fact]
    public void BrowseFolder_WhenFolderPicked_SetsSelectedFolder()
    {
        // Arrange
        _folderPicker.Setup(p => p.PickFolder(It.IsAny<string>())).Returns(@"C:\videos");
        var sut = CreateSut();

        // Act
        sut.BrowseFolderCommand.Execute(null);

        // Assert
        sut.SelectedFolder.Should().Be(@"C:\videos");
        sut.CanAnalyze.Should().BeTrue();
    }

    [Fact]
    public async Task AnalyzeAsync_PopulatesResultsAndCountsDamaged()
    {
        // Arrange
        _pipeline.Setup(p => p.RunAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IProgress<DiagnosisProgress>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { DamagedResult(), HealthyResult() });
        var sut = CreateSut();
        sut.SelectedFolder = @"C:\videos";

        // Act
        await sut.AnalyzeCommand.ExecuteAsync(null);

        // Assert
        sut.Results.Should().HaveCount(2);
        sut.Results.Count(r => r.IsDamaged).Should().Be(1);
        sut.StatusMessage.Should().Contain("손상 1개");
        sut.CanRecoverAll.Should().BeTrue();
    }

    [Fact]
    public async Task RecoverSelectedAsync_WhenSucceeds_UpdatesRowStatusToSuccess()
    {
        // Arrange
        var damaged = DamagedResult();
        _executor.Setup(e => e.ExecuteAsync(
                It.IsAny<string>(), It.IsAny<HealthReport>(), It.IsAny<CorruptionReport>(),
                It.IsAny<RecoveryPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RecoveryOutcome
            {
                OriginalPath = damaged.File.FullPath,
                Recovery = TestData.RecoverySuccess(level: RecoveryLevel.FrameRecovery),
                Validation = TestData.Validation(playable: true, fullyDecodable: true),
                Summary = "FrameRecovery 복구 성공"
            });
        var sut = CreateSut();
        var row = new DiagnosisRowViewModel(damaged);
        sut.Results.Add(row);
        sut.SelectedRow = row;

        // Act
        await sut.RecoverSelectedCommand.ExecuteAsync(null);

        // Assert
        row.RecoveryStatus.Should().Be("성공");
        row.RecoveredPath.Should().NotBeNullOrEmpty();
        _executor.Verify(e => e.ExecuteAsync(
            It.IsAny<string>(), It.IsAny<HealthReport>(), It.IsAny<CorruptionReport>(),
            It.IsAny<RecoveryPlan>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void CanRecoverSelected_WhenHealthyRowSelected_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var row = new DiagnosisRowViewModel(HealthyResult());
        sut.Results.Add(row);

        // Act
        sut.SelectedRow = row;

        // Assert
        sut.CanRecoverSelected.Should().BeFalse();
    }

    [Fact]
    public async Task RecoverAllDamagedAsync_RecoversOnlyDamagedRows()
    {
        // Arrange
        _executor.Setup(e => e.ExecuteAsync(
                It.IsAny<string>(), It.IsAny<HealthReport>(), It.IsAny<CorruptionReport>(),
                It.IsAny<RecoveryPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string path, HealthReport _, CorruptionReport _, RecoveryPlan _, CancellationToken _) =>
                new RecoveryOutcome
                {
                    OriginalPath = path,
                    Recovery = TestData.RecoverySuccess(original: path),
                    Summary = "성공"
                });
        var sut = CreateSut();
        sut.Results.Add(new DiagnosisRowViewModel(DamagedResult(@"C:\videos\bad1.mp4")));
        sut.Results.Add(new DiagnosisRowViewModel(HealthyResult(@"C:\videos\ok.mp4")));
        sut.Results.Add(new DiagnosisRowViewModel(DamagedResult(@"C:\videos\bad2.mp4")));

        // Act
        await sut.RecoverAllDamagedCommand.ExecuteAsync(null);

        // Assert — 손상 2개만 복구 호출.
        _executor.Verify(e => e.ExecuteAsync(
            It.IsAny<string>(), It.IsAny<HealthReport>(), It.IsAny<CorruptionReport>(),
            It.IsAny<RecoveryPlan>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        sut.StatusMessage.Should().Contain("2개 성공");
    }

    [Fact]
    public void CanSaveRecovered_WhenNoRecoveredResult_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var row = new DiagnosisRowViewModel(DamagedResult());
        sut.Results.Add(row);

        // Act
        sut.SelectedRow = row;

        // Assert — 아직 복구하지 않았으므로 저장 불가.
        sut.CanSaveRecovered.Should().BeFalse();
        sut.SaveRecoveredCommand.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void CanSaveRecovered_WhenRecoveredPathSet_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var row = new DiagnosisRowViewModel(DamagedResult()) { RecoveredPath = @"C:\ws\bad_recovered.mp4" };
        sut.Results.Add(row);

        // Act
        sut.SelectedRow = row;

        // Assert
        sut.CanSaveRecovered.Should().BeTrue();
        sut.SaveRecoveredCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void SaveRecovered_WhenSaved_CopiesViaServiceAndReportsPath()
    {
        // Arrange
        _fileSave.Setup(f => f.SaveCopy(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(@"D:\export\bad_recovered.mp4");
        var sut = CreateSut();
        var row = new DiagnosisRowViewModel(DamagedResult(@"C:\videos\bad.mp4")) { RecoveredPath = @"C:\ws\bad_recovered.mp4" };
        sut.Results.Add(row);
        sut.SelectedRow = row;

        // Act
        sut.SaveRecoveredCommand.Execute(null);

        // Assert
        _fileSave.Verify(f => f.SaveCopy(@"C:\ws\bad_recovered.mp4", It.Is<string>(n => n.Contains("recovered"))), Times.Once);
        sut.StatusMessage.Should().Contain("저장 완료");
    }

    [Fact]
    public void SaveRecovered_WhenCancelled_ReportsCancellation()
    {
        // Arrange
        _fileSave.Setup(f => f.SaveCopy(It.IsAny<string>(), It.IsAny<string>())).Returns((string?)null);
        var sut = CreateSut();
        var row = new DiagnosisRowViewModel(DamagedResult()) { RecoveredPath = @"C:\ws\bad_recovered.mp4" };
        sut.Results.Add(row);
        sut.SelectedRow = row;

        // Act
        sut.SaveRecoveredCommand.Execute(null);

        // Assert
        sut.StatusMessage.Should().Contain("취소");
    }

    [Fact]
    public void OpenRecoveredFolder_WhenRecovered_InvokesReveal()
    {
        // Arrange
        var sut = CreateSut();
        var row = new DiagnosisRowViewModel(DamagedResult()) { RecoveredPath = @"C:\ws\bad_recovered.mp4" };
        sut.Results.Add(row);
        sut.SelectedRow = row;

        // Act
        sut.OpenRecoveredFolderCommand.Execute(null);

        // Assert
        _fileSave.Verify(f => f.RevealInExplorer(@"C:\ws\bad_recovered.mp4"), Times.Once);
    }
}
