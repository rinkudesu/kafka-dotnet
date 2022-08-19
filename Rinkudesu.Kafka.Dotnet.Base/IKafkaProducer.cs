namespace Rinkudesu.Kafka.Dotnet.Base;

/// <summary>
/// Interface defining Kafka producer used for sending messages to the broker. Intended to be registered as a singleton instance.
/// </summary>
public interface IKafkaProducer : IDisposable
{
    /// <summary>
    /// Produces a single message to the broker.
    /// </summary>
    /// <param name="topic">Topic with which to send the message.</param>
    /// <param name="message">The message itself.</param>
    /// <param name="cancellationToken">Used for sending cancellation. Cancellation does not guarantee that the message will not be sent.</param>
    /// <remarks>
    /// Note that this uses thread pools, which is only really useful for sending Kafka messages in web-application scenarios.
    /// As such, this method (and this whole interface, really) is not intended for other usages.
    /// </remarks>
    /// <typeparam name="T">Type of the message to enqueue.</typeparam>
    Task Produce<T>(string topic, T message, CancellationToken cancellationToken = default) where T : GenericKafkaMessage;

    /// <summary>
    /// Produces multiple messages to the queue.
    /// </summary>
    /// <param name="topic">Topic under which to send the messages.</param>
    /// <param name="messages">The collection of messages.</param>
    /// <param name="waitTime">Maximum time to wait for all messages to be sent. If <see langword="null"/>, an arbitrary value will be used.</param>
    /// <typeparam name="T">Type of the messages to enqueue.</typeparam>
    /// <remarks>
    /// This message should be faster than <see cref="Produce{T}"/> as it does not wait for any single message to be enqueued.
    /// Due to this fact, it should only be used when speed is a major concern.
    /// If many messages need to be sent, but speed is not an issue in the relevant scenario, consider using <see cref="Produce{T}"/> in a loop instead.
    /// </remarks>
    void ProduceBulk<T>(string topic, IEnumerable<T> messages, TimeSpan? waitTime = null) where T : GenericKafkaMessage;
}
