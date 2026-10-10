using FluentAssertions;
using WiloNutrientSystem.Core.Parsing;

namespace WiloNutrientSystem.UnitTests.Core;

public class DeviceDataParserTests
{
    private readonly DeviceDataParser _parser = new();

    [Fact]
    public void Parse_ValidLine_ReturnsDeviceWithSensors()
    {
        var result = _parser.Parse("Pump01;RUN;Temp=42.5 C,Flow=12.3 m3h,Level=88 %");

        result.Should().NotBeNull();
        result!.DeviceId.Should().Be("Pump01");
        result.IsRunning.Should().BeTrue();
        result.Sensors.Should().HaveCount(3);
        result.FindSensor("Temp")!.Value.Should().Be(42.5);
        result.FindSensor("Temp")!.Unit.Should().Be("C");
        result.FindSensor("Level")!.Value.Should().Be(88);
    }

    [Fact]
    public void Parse_StopState_SetsIsRunningFalse()
    {
        var result = _parser.Parse("Pump01;STOP;Temp=10 C");
        result!.IsRunning.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("OnlyOneField")]
    public void Parse_InvalidLine_ReturnsNull(string line)
        => _parser.Parse(line).Should().BeNull();

    [Fact]
    public void Parse_SensorWithoutUnit_ParsesValueWithEmptyUnit()
    {
        var result = _parser.Parse("Pump01;RUN;Rpm=1500");
        var sensor = result!.FindSensor("Rpm");
        sensor!.Value.Should().Be(1500);
        sensor.Unit.Should().BeEmpty();
    }

    [Fact]
    public void Parse_MalformedSensorToken_IsSkipped()
    {
        var result = _parser.Parse("Pump01;RUN;Bad,Temp=20 C");
        result!.Sensors.Should().ContainSingle(s => s.Name == "Temp");
    }
}
