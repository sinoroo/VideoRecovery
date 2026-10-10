using System.IO;
using FluentAssertions;
using Moq;
using WiloNutrientSystem.App.Services;
using WiloNutrientSystem.App.ViewModels;
using WiloNutrientSystem.Infrastructure.Configuration;

namespace WiloNutrientSystem.UnitTests.ViewModels;

public class SettingsViewModelTests : IDisposable
{
    private readonly string _tempDir;
    private readonly IniConfigFileLoader _loader = new();
    private readonly Mock<IDialogService> _dialog = new();
    private readonly SettingsViewModel _sut;

    public SettingsViewModelTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "wilo_vm_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);

        _sut = new SettingsViewModel(_loader, _dialog.Object)
        {
            SerialConfigPath = Path.Combine(_tempDir, "serial.ini"),
            MqttConfigPath = Path.Combine(_tempDir, "mqtt.ini")
        };
    }

    [Fact]
    public void Validate_ReturnsError_WhenPortNameEmpty()
    {
        _sut.PortName = "";
        _sut.Validate().Should().Contain("PortName");
    }

    [Fact]
    public void Validate_ReturnsError_WhenBaudRateNonPositive()
    {
        _sut.BaudRate = 0;
        _sut.Validate().Should().Contain("BaudRate");
    }

    [Fact]
    public void Validate_ReturnsError_WhenPortOutOfRange()
    {
        _sut.Port = 70000;
        _sut.Validate().Should().Contain("Port");
    }

    [Fact]
    public void Validate_ReturnsEmpty_WhenValid()
    {
        _sut.PortName = "COM3";
        _sut.BaudRate = 9600;
        _sut.Port = 1883;
        _sut.Broker = "host";
        _sut.Validate().Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_PersistsFiles_WhenValid()
    {
        _sut.PortName = "COM5";
        _sut.BaudRate = 115200;
        _sut.Broker = "10.0.0.2";
        _sut.Port = 1883;

        await _sut.SaveCommand.ExecuteAsync(null);

        File.Exists(_sut.SerialConfigPath).Should().BeTrue();
        File.Exists(_sut.MqttConfigPath).Should().BeTrue();
        _sut.StatusMessage.Should().Be("Settings saved.");
        _dialog.Verify(d => d.ShowInfoAsync("Settings", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_ShowsValidationDialog_WhenInvalid()
    {
        _sut.PortName = "";

        await _sut.SaveCommand.ExecuteAsync(null);

        File.Exists(_sut.SerialConfigPath).Should().BeFalse();
        _dialog.Verify(d => d.ShowInfoAsync("Validation error", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Load_ReadsPersistedValues()
    {
        File.WriteAllText(_sut.SerialConfigPath, "[Serial]\nPortName=COM8\nBaudRate=4800\n");
        File.WriteAllText(_sut.MqttConfigPath, "[MQTT]\nBroker=h\nPort=1884\n");

        _sut.LoadCommand.Execute(null);

        _sut.PortName.Should().Be("COM8");
        _sut.BaudRate.Should().Be(4800);
        _sut.Broker.Should().Be("h");
        _sut.Port.Should().Be(1884);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }
}
