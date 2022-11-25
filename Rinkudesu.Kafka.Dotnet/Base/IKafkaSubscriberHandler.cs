using Microsoft.Extensions.DependencyInjection;

namespace Rinkudesu.Kafka.Dotnet.Base;

/// <summary>
/// Interface for handling kafka listener subscriptions.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IKafkaSubscriberHandler<T> where T : GenericKafkaMessage
{
    /// <summary>
    /// Topic for which the subscriber is listening.
    /// </summary>
    string Topic { get; }

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <returns><see langword="true"/> if handled correctly.</returns>
    Task<bool> Handle(T rawMessage, CancellationToken cancellationToken = default);
    /// <summary>
    /// Parses the message from string to the required type.
    /// </summary>
    T Parse(string rawMessage);

    /// <summary>
    /// Sets scope for use in aspnet DI.
    /// </summary>
    IKafkaSubscriberHandler<T> SetScope(IServiceScope serviceScope);
}
