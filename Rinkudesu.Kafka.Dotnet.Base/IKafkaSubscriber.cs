namespace Rinkudesu.Kafka.Dotnet.Base;

/// <summary>
/// Interface defining Kafka consumer/subscriber. It's intended to be used as a singleton-type object running as a background service.
/// </summary>
public interface IKafkaSubscriber<T> : IAsyncDisposable where T : GenericKafkaMessage
{
    /// <summary>
    /// Subscribes to a topic defined by <paramref name="newHandler"/>
    /// </summary>
    /// <remarks>
    /// Subscription by itself doesn't start message handling. For that, you need to call <see cref="BeginHandle"/>.
    /// </remarks>
    void Subscribe(IKafkaSubscriberHandler<T> newHandler);
    /// <summary>
    /// Removes the subscription and stops message handling.
    /// </summary>
    Task<bool> Unsubscribe();

    /// <summary>
    /// Starts message handling.
    /// </summary>
    void BeginHandle(CancellationToken cancellationToken);
    /// <summary>
    /// Cancels message handling and waits for the processing to finish.
    /// </summary>
    Task StopHandle();
}
