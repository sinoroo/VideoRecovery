using FluentAssertions;
using WiloNutrientSystem.Domain.Models;

namespace WiloNutrientSystem.UnitTests.Domain;

public class DeviceDataTests
{
    [Fact]
    public void FindSensor_ReturnsMatch_CaseInsensitive()
    {
        var device = new DeviceData
        {
            Sensors = new List<SensorData>
            {
                new() { Name = "Temp", Value = 42 },
                new() { Name = "Flow", Value = 12 }
            }
        };

        device.FindSensor("temp").Should().NotBeNull();
        device.FindSensor("temp")!.Value.Should().Be(42);
    }

    [Fact]
    public void FindSensor_ReturnsNull_WhenMissing()
    {
        var device = new DeviceData();
        device.FindSensor("nope").Should().BeNull();
    }

    [Fact]
    public void DefaultDeviceId_IsPump01()
        => new DeviceData().DeviceId.Should().Be("Pump01");
}
