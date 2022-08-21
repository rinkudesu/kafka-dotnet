using System.Diagnostics.CodeAnalysis;

namespace Rinkudesu.Kafka.Dotnet.Base;

[ExcludeFromCodeCoverage]
public class KafkaConfigurationProvider
{
    /// <summary>
    /// host:port address of the Kafka broker
    /// </summary>
    public string ServerAddress { get; }

    /// <summary>
    /// Username used to authenticate with the broker
    /// </summary>
    public string? User { get; }

    /// <summary>
    /// Password used to authenticate with the broker
    /// </summary>
    public string? Password { get; }

    /// <summary>
    /// Kafka broker client id
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// Consumer group id. Required when creating consumers.
    /// </summary>
    public string? ConsumerGroupId { get; }

    public KafkaConfigurationProvider(string serverAddress, string? user, string? password, string clientId, string? consumerGroupId = null)
    {
        ServerAddress = serverAddress;
        User = user;
        Password = password;
        ClientId = clientId;
        ConsumerGroupId = consumerGroupId;
    }

    public static KafkaConfigurationProvider ReadFromEnv()
    {
        var serverAddress = Environment.GetEnvironmentVariable("RINKU_KAFKA_ADDRESS");
        var user = Environment.GetEnvironmentVariable("RINKU_KAFKA_USER");
        var password = Environment.GetEnvironmentVariable("RINKU_KAFKA_PASSWORD");
        var clientId = Environment.GetEnvironmentVariable("RINKU_KAFKA_CLIENT_ID");
        var consumerGroupId = Environment.GetEnvironmentVariable("RINKU_KAFKA_CONSUMER_GROUP_ID");

        if (string.IsNullOrWhiteSpace(serverAddress) || string.IsNullOrWhiteSpace(clientId)) throw new InvalidOperationException("Env variables are not set");

        return new KafkaConfigurationProvider(serverAddress, user, password, clientId, consumerGroupId);
    }
}
