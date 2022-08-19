using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rinkudesu.Kafka.Dotnet.Base;

namespace Rinkudesu.Kafka.Dotnet.IntegrationTests;

public class MockMessageHandler : IKafkaSubscriberHandler<MockKafkaMessage>
{
    public bool Result { get; set; } = true;
    public int HandleCount { get; private set; }
    public MockKafkaMessage? LastMessage { get; private set; }

    public string Topic { get; } = Guid.NewGuid().ToString();

    public Task<bool> Handle(MockKafkaMessage rawMessage, CancellationToken cancellationToken = default)
    {
        HandleCount++;
        LastMessage = rawMessage;
        return Task.FromResult(Result);
    }

    public MockKafkaMessage Parse(string rawMessage) => GenericKafkaMessage.Deserialise<MockKafkaMessage>(rawMessage)!;

    public IKafkaSubscriberHandler<MockKafkaMessage> SetScope(IServiceScope serviceScope) => this;
}
