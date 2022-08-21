using Microsoft.Extensions.DependencyInjection;

namespace Rinkudesu.Kafka.Dotnet.Base;

public interface IKafkaSubscriberHandler<T> where T : GenericKafkaMessage
{
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
