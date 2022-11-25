using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Confluent.Kafka;
using Rinkudesu.Kafka.Dotnet.Base;

namespace Rinkudesu.Kafka.Dotnet;

/// <summary>
/// Provides additional functionality to <see cref="GenericKafkaMessage"/>.
/// </summary>
public static class KafkaMessageExtensions
{
    /// <summary>
    /// Generates message for sending from <see cref="GenericKafkaMessage"/> derivative.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static Message<Null, string> GetMessage(this GenericKafkaMessage message, JsonSerializerOptions? options = null) => new() { Value = message.GetJson(options) };
}
