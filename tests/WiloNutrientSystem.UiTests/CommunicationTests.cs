using WiloNutrientSystem.UiTests.Infrastructure;

namespace WiloNutrientSystem.UiTests;

[Collection("App collection")]
public class CommunicationTests(ApplicationFixture fixture)
{
    [Fact]
    public void Serial_ConnectAndDisconnectButtons_AreOperable()
    {
        fixture.MainWindow.ClickById("NavCommunication");
        fixture.MainWindow.WaitFor("CommunicationView").Should().NotBeNull();

        // Connect will fail (no real COM port) but must not crash; state text updates.
        fixture.MainWindow.ClickById("SerialConnectButton");
        fixture.MainWindow.Require("SerialStateText").Should().NotBeNull();

        fixture.MainWindow.ClickById("SerialDisconnectButton");
        fixture.Capture("communication-serial");
    }

    [Fact]
    public void Mqtt_ConnectAndDisconnectButtons_AreOperable()
    {
        fixture.MainWindow.ClickById("NavCommunication");

        fixture.MainWindow.ClickById("MqttConnectButton");
        fixture.MainWindow.Require("MqttStateText").Should().NotBeNull();

        fixture.MainWindow.ClickById("MqttDisconnectButton");
        fixture.Capture("communication-mqtt");
    }
}
