using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Confluent.Kafka;
using Rinkudesu.Kafka.Dotnet.Base;

namespace Rinkudesu.Kafka.Dotnet;

public static class KafkaMessageExtensions
{
    [ExcludeFromCodeCoverage]
    public static Message<Null, string> GetMessage(this GenericKafkaMessage message, JsonSerializerOptions? options = null) => new() { Value = message.GetJson(options) };
}
