using FlaUI.Core.AutomationElements;
using WiloNutrientSystem.UiTests.Infrastructure;

namespace WiloNutrientSystem.UiTests;

[Collection("App collection")]
public class SettingsTests(ApplicationFixture fixture)
{
    [Fact]
    public void Settings_SaveShowsConfirmationDialog_ThenClose()
    {
        fixture.MainWindow.ClickById("NavSettings");
        fixture.MainWindow.WaitFor("SettingsView").Should().NotBeNull();

        // Edit a value then save.
        var portBox = fixture.MainWindow.Require("PortNameBox").AsTextBox();
        portBox.Text = "COM4";

        fixture.MainWindow.ClickById("SaveButton");

        // Save shows an info dialog with an OK button.
        var ok = fixture.MainWindow.WaitFor("DialogOkButton", 8000);
        ok.Should().NotBeNull();
        fixture.Capture("settings-save-dialog");

        ok!.AsButton().Invoke();

        // Dialog dismissed and status text updated.
        var status = fixture.MainWindow.Require("StatusMessageText");
        status.Name.Should().NotBeNull();
    }

    [Fact]
    public void Settings_LoadReadsConfiguration()
    {
        fixture.MainWindow.ClickById("NavSettings");
        fixture.MainWindow.ClickById("LoadButton");

        var broker = fixture.MainWindow.Require("BrokerBox").AsTextBox();
        broker.Text.Should().NotBeNullOrEmpty();
    }
}
