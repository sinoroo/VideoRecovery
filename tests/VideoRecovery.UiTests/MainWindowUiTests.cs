using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FluentAssertions;
using Xunit;

namespace VideoRecovery.UiTests;

/// <summary>
/// 메인 윈도우의 핵심 UI 계약을 FlaUI(UIA3)로 검증한다. 요소는 AutomationId로만 조회한다.
/// 실제 분석/복구(ffmpeg 호출)는 트리거하지 않는다 — UI 가용성/배선만 확인한다.
/// </summary>
[Collection("App collection")]
public sealed class MainWindowUiTests
{
    private readonly ApplicationFixture _fixture;

    public MainWindowUiTests(ApplicationFixture fixture) => _fixture = fixture;

    private Window MainWindow => _fixture.App.GetMainWindow(_fixture.Automation, TimeSpan.FromSeconds(20));

    private AutomationElement? Find(string automationId)
        => MainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));

    [Fact]
    public void MainWindow_Loads_WithExpectedTitleRegion()
    {
        // Arrange & Act
        var window = MainWindow;

        // Assert
        window.Should().NotBeNull();
        _fixture.Capture("main-window-loaded");
    }

    [Theory]
    [InlineData("BrowseButton")]
    [InlineData("FolderTextBox")]
    [InlineData("RecursiveCheckBox")]
    [InlineData("AnalyzeButton")]
    [InlineData("CancelButton")]
    [InlineData("RecoverSelectedButton")]
    [InlineData("RecoverAllButton")]
    [InlineData("ProgressBar")]
    [InlineData("StatusText")]
    [InlineData("ResultsGrid")]
    [InlineData("ExitButton")]
    public void MainWindow_ContainsControl(string automationId)
    {
        // Act
        var element = Find(automationId);

        // Assert
        element.Should().NotBeNull($"컨트롤 '{automationId}' 이 존재해야 합니다.");
    }

    [Fact]
    public void FolderTextBox_WhenPathEntered_EnablesAnalyzeButton()
    {
        // Arrange
        var textBox = Find("FolderTextBox")!.AsTextBox();
        var analyze = Find("AnalyzeButton")!.AsButton();

        // Act
        textBox.Text = @"C:\some\folder\path";

        // Assert — SelectedFolder 바인딩으로 CanAnalyze가 true가 되어 버튼이 활성화된다.
        analyze.IsEnabled.Should().BeTrue();
        _fixture.Capture("analyze-enabled");
    }

    [Fact]
    public void RecoverButtons_InitiallyDisabled_NoDamagedResults()
    {
        // Arrange & Act
        var recoverAll = Find("RecoverAllButton")!.AsButton();
        var recoverSelected = Find("RecoverSelectedButton")!.AsButton();

        // Assert — 결과가 없으므로 복구 버튼은 비활성.
        recoverAll.IsEnabled.Should().BeFalse();
        recoverSelected.IsEnabled.Should().BeFalse();
    }
}
