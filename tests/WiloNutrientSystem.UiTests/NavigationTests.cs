using WiloNutrientSystem.UiTests.Infrastructure;

namespace WiloNutrientSystem.UiTests;

[Collection("App collection")]
public class NavigationTests(ApplicationFixture fixture)
{
    [Fact]
    public void NavigateTo_Communication_ShowsCommunicationView()
    {
        fixture.MainWindow.ClickById("NavCommunication");
        fixture.MainWindow.WaitFor("CommunicationView").Should().NotBeNull();
        fixture.Capture("nav-communication");
    }

    [Fact]
    public void NavigateTo_Settings_ShowsSettingsView()
    {
        fixture.MainWindow.ClickById("NavSettings");
        fixture.MainWindow.WaitFor("SettingsView").Should().NotBeNull();
        fixture.Capture("nav-settings");
    }

    [Fact]
    public void NavigateTo_Dashboard_ShowsDashboardView()
    {
        fixture.MainWindow.ClickById("NavDashboard");
        fixture.MainWindow.WaitFor("DashboardView").Should().NotBeNull();
        fixture.Capture("nav-dashboard");
    }
}
