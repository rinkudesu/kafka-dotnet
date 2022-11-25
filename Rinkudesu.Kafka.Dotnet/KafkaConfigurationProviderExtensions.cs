using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using Rinkudesu.Kafka.Dotnet.Base;

namespace Rinkudesu.Kafka.Dotnet;

/// <summary>
/// Provides extension methods for <see cref="KafkaConfigurationProvider"/>
/// </summary>
[ExcludeFromCodeCoverage]
public static class KafkaConfigurationProviderExtensions
{
    /// <summary>
    /// Generates Kafka connection config used by <see cref="IKafkaProducer"/>.
    /// </summary>
    public static ProducerConfig GetProducerConfig(this KafkaConfigurationProvider configurationProvider)
        => SetUserPassword(new ProducerConfig
        {
            BootstrapServers = configurationProvider.ServerAddress,
            ClientId = configurationProvider.ClientId
        }, configurationProvider);

    /// <summary>
    /// Generates Kafka connection config used by <see cref="IKafkaSubscriber{T}"/>.
    /// </summary>
    public static ConsumerConfig GetConsumerConfig(this KafkaConfigurationProvider configurationProvider) =>
        SetUserPassword(new ConsumerConfig
        {
            BootstrapServers = configurationProvider.ServerAddress,
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            ClientId = configurationProvider.ClientId,
            GroupId = configurationProvider.ConsumerGroupId ?? throw new InvalidOperationException("Consumer group is must be set when creating the consumer"),
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
        }, configurationProvider);

    private static T SetUserPassword<T>(T config, KafkaConfigurationProvider configurationProvider) where T : ClientConfig
    {
        if (configurationProvider.User is null || configurationProvider.Password is null) return config;

        config.SaslUsername = configurationProvider.User;
        config.SaslPassword = configurationProvider.Password;
        config.SaslMechanism = SaslMechanism.Plain;
        config.SecurityProtocol = SecurityProtocol.SaslPlaintext;
        return config;
    }
}
