using System.IO;
using FluentAssertions;
using WiloNutrientSystem.Domain.Models;
using WiloNutrientSystem.Infrastructure.Configuration;

namespace WiloNutrientSystem.UnitTests.Infrastructure;

public class IniConfigFileLoaderTests : IDisposable
{
    private readonly IniConfigFileLoader _loader = new();
    private readonly string _tempDir;

    public IniConfigFileLoaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "wilo_tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public void LoadSerialConfig_ReadsAllFields()
    {
        var path = Path.Combine(_tempDir, "serial.ini");
        File.WriteAllText(path,
            "[Serial]\nPortName=COM7\nBaudRate=57600\nDataBits=7\nParity=Even\nStopBits=Two\n");

        var config = _loader.LoadSerialConfig(path);

        config.PortName.Should().Be("COM7");
        config.BaudRate.Should().Be(57600);
        config.DataBits.Should().Be(7);
        config.Parity.Should().Be("Even");
        config.StopBits.Should().Be("Two");
    }

    [Fact]
    public void LoadMqttConfig_ReadsAllFields()
    {
        var path = Path.Combine(_tempDir, "mqtt.ini");
        File.WriteAllText(path,
            "[MQTT]\nBroker=192.168.1.5\nPort=8883\nClientId=abc\nUserName=u\nPassword=p\n");

        var config = _loader.LoadMqttConfig(path);

        config.Broker.Should().Be("192.168.1.5");
        config.Port.Should().Be(8883);
        config.ClientId.Should().Be("abc");
        config.UserName.Should().Be("u");
        config.Password.Should().Be("p");
    }

    [Fact]
    public void LoadSerialConfig_MissingFile_ReturnsDefaults()
    {
        var config = _loader.LoadSerialConfig(Path.Combine(_tempDir, "missing.ini"));
        config.PortName.Should().Be("COM1");
        config.BaudRate.Should().Be(9600);
    }

    [Fact]
    public void Save_ThenLoad_RoundTripsSerial()
    {
        var path = Path.Combine(_tempDir, "roundtrip_serial.ini");
        var original = new SerialConfig { PortName = "COM9", BaudRate = 19200, Parity = "Odd" };

        _loader.SaveSerialConfig(path, original);
        var loaded = _loader.LoadSerialConfig(path);

        loaded.PortName.Should().Be("COM9");
        loaded.BaudRate.Should().Be(19200);
        loaded.Parity.Should().Be("Odd");
    }

    [Fact]
    public void Parse_IgnoresCommentsAndBlankLines()
    {
        var path = Path.Combine(_tempDir, "comments.ini");
        File.WriteAllText(path, "; comment\n\n[MQTT]\n# another\nBroker=host\nPort=1883\n");

        var config = _loader.LoadMqttConfig(path);
        config.Broker.Should().Be("host");
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }
}
