using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Rinkudesu.Kafka.Dotnet.Base;

[ExcludeFromCodeCoverage]
public abstract class GenericKafkaMessage
{
    public string GetJson(JsonSerializerOptions? options = null) => JsonSerializer.Serialize<object>(this, options);
    public static T? Deserialise<T>(string kafkaData, JsonSerializerOptions? options = null) => JsonSerializer.Deserialize<T>(kafkaData, options);
}
