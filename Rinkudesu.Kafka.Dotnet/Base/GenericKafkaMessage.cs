using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Rinkudesu.Kafka.Dotnet.Base;

/// <summary>
/// Class meant to be base for all Kafka messages.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class GenericKafkaMessage
{
    /// <summary>
    /// Returns JSON-formatted message.
    /// </summary>
    /// <param name="options">Options used during JSON serialisation</param>
    /// <returns></returns>
    public string GetJson(JsonSerializerOptions? options = null) => JsonSerializer.Serialize<object>(this, options);
    /// <summary>
    /// Recreates the messages from JSON read from Kafka.
    /// </summary>
    /// <param name="kafkaData">Data received from queue</param>
    /// <param name="options">Options used during JSON deserialisation</param>
    /// <typeparam name="T">Type of the message.</typeparam>
    /// <returns>Deserialised message, or <c>null</c> if failed to parse.</returns>
    public static T? Deserialise<T>(string kafkaData, JsonSerializerOptions? options = null) => JsonSerializer.Deserialize<T>(kafkaData, options);
}
