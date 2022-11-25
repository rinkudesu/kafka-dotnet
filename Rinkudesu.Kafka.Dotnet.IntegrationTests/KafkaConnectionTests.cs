using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Rinkudesu.Kafka.Dotnet.Base;
using Xunit;

namespace Rinkudesu.Kafka.Dotnet.IntegrationTests;

public sealed class KafkaConnectionTests : IAsyncDisposable
{
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory = new();

    private readonly MockMessageHandler _messageHandler = new();
    private readonly IKafkaProducer _producer;
    private readonly IKafkaSubscriber<MockKafkaMessage> _subscriber;

    public KafkaConnectionTests()
    {
        var config = new KafkaConfigurationProvider("localhost:9092", null, null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        _producer = new KafkaProducer(config);
        _subscriber = new KafkaSubscriber<MockKafkaMessage>(config, _mockScopeFactory.Object, new NullLogger<KafkaSubscriber<MockKafkaMessage>>());
    }

    [Fact]
    public async Task KafkaConnection_ConnectsProducerAndConsumer()
    {
        using var handleCancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        var mockMessage = new MockKafkaMessage { MockMessage = "Hello there" };

        _subscriber.Subscribe(_messageHandler);
        _subscriber.BeginHandle(handleCancellationToken.Token);
        await _producer.Produce(_messageHandler.Topic, mockMessage, handleCancellationToken.Token);
        await Task.Delay(TimeSpan.FromSeconds(10), CancellationToken.None); //wait for messages to definitely circulate
        await _subscriber.Unsubscribe();

        Assert.Equal(1, _messageHandler.HandleCount);
        Assert.NotNull(_messageHandler.LastMessage);
        Assert.Equal(mockMessage.MockMessage, _messageHandler.LastMessage.MockMessage);
    }

    [Theory]
    [InlineData(100)]
    public async Task KafkaConnection_BulkSendMessages_AllProcessed(int msgCount)
    {
        using var handleCancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        var messages = new List<MockKafkaMessage>(msgCount);
        for (int i = 0; i < msgCount; i++)
        {
            messages.Add(new MockKafkaMessage { MockMessage = i.ToString() });
        }

        _subscriber.Subscribe(_messageHandler);
        _subscriber.BeginHandle(handleCancellationToken.Token);
        var result = _producer.ProduceBulk(_messageHandler.Topic, messages, TimeSpan.FromSeconds(5));
        await Task.Delay(TimeSpan.FromSeconds(10), CancellationToken.None); //wait for messages to definitely circulate
        await _subscriber.Unsubscribe();

        Assert.Equal(msgCount, _messageHandler.HandleCount);
        Assert.True(result);
    }

    public async ValueTask DisposeAsync()
    {
        _producer.Dispose();
        await _subscriber.DisposeAsync();
    }
}
