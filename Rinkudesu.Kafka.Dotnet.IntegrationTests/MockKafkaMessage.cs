using Rinkudesu.Kafka.Dotnet.Base;

namespace Rinkudesu.Kafka.Dotnet.IntegrationTests;

public class MockKafkaMessage : GenericKafkaMessage
{
    public string MockMessage { get; set; }
}
