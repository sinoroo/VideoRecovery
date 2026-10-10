using Microsoft.Extensions.Logging.Abstractions;
using FluentAssertions;
using Moq;
using WiloNutrientSystem.Core.Abstractions;
using WiloNutrientSystem.Core.Events;
using WiloNutrientSystem.Core.Managers;
using WiloNutrientSystem.Domain.Models;

namespace WiloNutrientSystem.UnitTests.Core;

public class CommunicationManagerTests
{
    private readonly Mock<ISerialDeviceService> _serial = new();
    private readonly Mock<IMqttService> _mqtt = new();
    private readonly Mock<IDeviceDataParser> _parser = new();
    private readonly CommunicationManager _sut;

    public CommunicationManagerTests()
        => _sut = new CommunicationManager(_serial.Object, _mqtt.Object, _parser.Object,
            NullLogger<CommunicationManager>.Instance);

    [Fact]
    public async Task ConnectMqtt_ConnectsAndSubscribes()
    {
        var config = new MqttConfig { SubscribeTopic = "Cmd" };

        await _sut.ConnectMqttAsync(config);

        _mqtt.Verify(m => m.ConnectAsync(config, It.IsAny<CancellationToken>()), Times.Once);
        _mqtt.Verify(m => m.SubscribeAsync("Cmd", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SerialData_IsParsed_AndPublished()
    {
        var device = new DeviceData { DeviceId = "P1" };
        _parser.Setup(p => p.Parse("raw")).Returns(device);
        await _sut.ConnectMqttAsync(new MqttConfig { PublishTopic = "Data", SubscribeTopic = "Cmd" });

        DeviceData? published = null;
        _sut.DeviceDataUpdated += (_, d) => published = d;

        _serial.Raise(s => s.DataReceived += null, _serial.Object, new DataReceivedEventArgs("raw"));

        published.Should().Be(device);
        _mqtt.Verify(m => m.PublishAsync("Data", "raw", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void MqttCommand_IsForwardedToSerial()
    {
        _mqtt.Raise(m => m.CommandReceived += null, _mqtt.Object, new DataReceivedEventArgs("START"));

        _serial.Verify(s => s.SendAsync("START", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void UnparseablePayload_IsNotPublished()
    {
        _parser.Setup(p => p.Parse(It.IsAny<string>())).Returns((DeviceData?)null);

        _serial.Raise(s => s.DataReceived += null, _serial.Object, new DataReceivedEventArgs("junk"));

        _mqtt.Verify(m => m.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
