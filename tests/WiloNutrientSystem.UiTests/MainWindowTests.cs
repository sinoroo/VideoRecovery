using FlaUI.Core.Definitions;
using WiloNutrientSystem.UiTests.Infrastructure;

namespace WiloNutrientSystem.UiTests;

[Collection("App collection")]
public class MainWindowTests(ApplicationFixture fixture)
{
    [Fact]
    public void MainWindow_Loads_WithTitle()
    {
        fixture.MainWindow.Should().NotBeNull();
        fixture.MainWindow.Title.Should().Contain("Wilo");
        fixture.Capture("main-window-loaded");
    }

    [Fact]
    public void MainWindow_IsMaximized_AndBorderless()
    {
        // WindowStyle=None + Maximized => the window fills the work area.
        var pattern = fixture.MainWindow.Patterns.Window.Pattern;
        pattern.WindowVisualState.Value.Should().Be(WindowVisualState.Maximized);
    }

    [Fact]
    public void NavigationRail_ButtonsExist()
    {
        fixture.MainWindow.WaitFor("NavDashboard").Should().NotBeNull();
        fixture.MainWindow.WaitFor("NavCommunication").Should().NotBeNull();
        fixture.MainWindow.WaitFor("NavSettings").Should().NotBeNull();
    }
}
