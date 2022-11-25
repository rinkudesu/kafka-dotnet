using Confluent.Kafka;
using Rinkudesu.Kafka.Dotnet.Base;
using Rinkudesu.Kafka.Dotnet.Exceptions;

namespace Rinkudesu.Kafka.Dotnet;

/// <inheritdoc/>
public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    /// <summary>
    /// Creates new <see cref="KafkaProducer"/> based on provided <see cref="KafkaConfigurationProvider"/>.
    /// </summary>
    /// <param name="kafkaConfig"></param>
    public KafkaProducer(KafkaConfigurationProvider kafkaConfig)
    {
        _producer = new ProducerBuilder<Null, string>(kafkaConfig.GetProducerConfig()).Build();
    }

    /// <inheritdoc cref="IKafkaProducer.Produce{T}"/>
    /// <exception cref="KafkaProduceException">Thrown when message sending failed.</exception>
    public virtual async Task Produce<T>(string topic, T message, CancellationToken cancellationToken = default) where T : GenericKafkaMessage
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, message.GetMessage(), cancellationToken).ConfigureAwait(false);
            if (result.Status != PersistenceStatus.Persisted) throw new KafkaProduceException("Failed to persist message.");
        }
        catch (ProduceException<Null, string> e)
        {
            throw new KafkaProduceException(e);
        }
    }

    /// <inheritdoc cref="IKafkaProducer.ProduceBulk{T}"/>
    /// <exception cref="AggregateException">
    /// Thrown when sending of at least one message has failed. Will contain <see cref="KafkaProduceException"/> thrown during sending.
    /// Note that all messages will attempt to send, regardless of when the first exception was thrown.
    /// </exception>
    public virtual bool ProduceBulk<T>(string topic, IEnumerable<T> messages, TimeSpan? waitTime = null) where T : GenericKafkaMessage
    {
        bool flushed;
        var exceptions = new LinkedList<KafkaProduceException>();
        try
        {
            foreach (var message in messages)
            {
                try
                {
                    _producer.Produce(topic, message.GetMessage());
                }
                catch (ProduceException<Null, string> e)
                {
                    exceptions.AddLast(new KafkaProduceException(e));
                }
            }
        }
        finally
        {
            flushed = _producer.Flush(waitTime ?? TimeSpan.FromSeconds(10)) == 0;
        }

        if (exceptions.Any()) throw new AggregateException(exceptions);
        return flushed;
    }

    /// <summary>
    /// IDisposable pattern implementation.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _producer.Dispose();
        }
    }

    /// <summary>
    /// IDisposable pattern implementation.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
