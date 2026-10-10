using FluentAssertions;
using Moq;
using WiloNutrientSystem.App.ViewModels;
using WiloNutrientSystem.Core.Abstractions;
using WiloNutrientSystem.Core.Events;
using WiloNutrientSystem.Core.Managers;
using WiloNutrientSystem.Domain.Enums;
using WiloNutrientSystem.Domain.Models;

namespace WiloNutrientSystem.UnitTests.ViewModels;

public class CommunicationViewModelTests
{
    private readonly Mock<ICommunicationManager> _manager = new();
    private readonly Mock<IConfigFileLoader> _loader = new();
    private readonly CommunicationViewModel _sut;

    public CommunicationViewModelTests()
    {
        _loader.Setup(l => l.LoadSerialConfig(It.IsAny<string>())).Returns(new SerialConfig());
        _loader.Setup(l => l.LoadMqttConfig(It.IsAny<string>())).Returns(new MqttConfig());
        _sut = new CommunicationViewModel(_manager.Object, _loader.Object);
    }

    [Fact]
    public async Task ConnectSerialCommand_InvokesManager()
    {
        await _sut.ConnectSerialCommand.ExecuteAsync(null);
        _manager.Verify(m => m.ConnectSerialAsync(It.IsAny<SerialConfig>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DisconnectSerialCommand_InvokesManager()
    {
        await _sut.DisconnectSerialCommand.ExecuteAsync(null);
        _manager.Verify(m => m.DisconnectSerialAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConnectMqttCommand_InvokesManager()
    {
        await _sut.ConnectMqttCommand.ExecuteAsync(null);
        _manager.Verify(m => m.ConnectMqttAsync(It.IsAny<MqttConfig>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConnectSerial_CapturesError_WhenManagerThrows()
    {
        _manager.Setup(m => m.ConnectSerialAsync(It.IsAny<SerialConfig>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        await _sut.ConnectSerialCommand.ExecuteAsync(null);

        _sut.LastError.Should().Be("boom");
    }

    [Fact]
    public void SerialStateChanged_UpdatesState()
    {
        _manager.Raise(m => m.SerialStateChanged += null,
            _manager.Object, new ConnectionStateChangedEventArgs(ConnectionState.Connected));

        _sut.SerialState.Should().Be(ConnectionState.Connected);
        _sut.IsSerialConnected.Should().BeTrue();
    }

    [Fact]
    public void MqttStateChanged_UpdatesState()
    {
        _manager.Raise(m => m.MqttStateChanged += null,
            _manager.Object, new ConnectionStateChangedEventArgs(ConnectionState.Connected));

        _sut.MqttState.Should().Be(ConnectionState.Connected);
        _sut.IsMqttConnected.Should().BeTrue();
    }
}
