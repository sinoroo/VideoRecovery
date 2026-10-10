using FluentAssertions;
using Moq;
using WiloNutrientSystem.App.ViewModels;
using WiloNutrientSystem.Core.Managers;
using WiloNutrientSystem.Domain.Models;

namespace WiloNutrientSystem.UnitTests.ViewModels;

public class DashboardViewModelTests
{
    private readonly Mock<ICommunicationManager> _manager = new();
    private readonly DashboardViewModel _sut;

    public DashboardViewModelTests() => _sut = new DashboardViewModel(_manager.Object);

    [Fact]
    public void Apply_UpdatesStateAndSensors()
    {
        var data = new DeviceData
        {
            DeviceId = "Pump42",
            IsRunning = true,
            Sensors = new List<SensorData>
            {
                new() { Name = "Temp", Value = 30 },
                new() { Name = "Flow", Value = 5 }
            }
        };

        _sut.Apply(data);

        _sut.DeviceId.Should().Be("Pump42");
        _sut.IsRunning.Should().BeTrue();
        _sut.Sensors.Should().HaveCount(2);
    }

    [Fact]
    public void DeviceDataUpdated_Event_AppliesSnapshot()
    {
        var data = new DeviceData { DeviceId = "PumpX", IsRunning = false };

        _manager.Raise(m => m.DeviceDataUpdated += null, _manager.Object, data);

        _sut.DeviceId.Should().Be("PumpX");
    }

    [Fact]
    public void Apply_Twice_ReplacesSensors()
    {
        _sut.Apply(new DeviceData { Sensors = new List<SensorData> { new() { Name = "A" } } });
        _sut.Apply(new DeviceData { Sensors = new List<SensorData> { new() { Name = "B" } } });

        _sut.Sensors.Should().ContainSingle(s => s.Name == "B");
    }

    [Fact]
    public void Dispose_UnsubscribesFromManager()
    {
        _sut.Dispose();
        _manager.Raise(m => m.DeviceDataUpdated += null, _manager.Object,
            new DeviceData { DeviceId = "ShouldNotApply" });

        _sut.DeviceId.Should().NotBe("ShouldNotApply");
    }
}
