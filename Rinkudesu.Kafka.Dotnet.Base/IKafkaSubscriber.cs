namespace Rinkudesu.Kafka.Dotnet.Base;

/// <summary>
/// Interface defining Kafka consumer/subscriber. It's intended to be used as a singleton-type object running as a background service.
/// </summary>
public interface IKafkaSubscriber<T> : IAsyncDisposable where T : GenericKafkaMessage
{
    void Subscribe(IKafkaSubscriberHandler<T> newHandler);
    Task<bool> Unsubscribe();

    void BeginHandle(CancellationToken cancellationToken);
    Task StopHandle();
}
