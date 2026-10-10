using FluentAssertions;
using WiloNutrientSystem.Domain.Enums;
using WiloNutrientSystem.Domain.Models;

namespace WiloNutrientSystem.UnitTests.Domain;

public class ModelDefaultsTests
{
    [Fact]
    public void SerialConfig_ToString_FormatsCorrectly()
    {
        var config = new SerialConfig
        {
            PortName = "COM3",
            BaudRate = 115200,
            DataBits = 8,
            Parity = "None",
            StopBits = "One"
        };

        config.ToString().Should().Be("COM3 @ 115200 8NOne");
    }

    [Fact]
    public void MqttConfig_ToString_FormatsBrokerAndClient()
    {
        var config = new MqttConfig { Broker = "10.0.0.1", Port = 1883, ClientId = "c1" };
        config.ToString().Should().Be("10.0.0.1:1883 (c1)");
    }

    [Fact]
    public void SensorData_ToString_IncludesUnit()
    {
        var sensor = new SensorData { Name = "Temp", Value = 42.5, Unit = "C" };
        sensor.ToString().Should().Be("Temp=42.5C");
    }

    [Fact]
    public void AlarmData_Defaults_AreInfoAndNotAcknowledged()
    {
        var alarm = new AlarmData();
        alarm.Severity.Should().Be(AlarmSeverity.Info);
        alarm.IsAcknowledged.Should().BeFalse();
    }
}
