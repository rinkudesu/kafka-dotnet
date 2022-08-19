using Confluent.Kafka;
using Rinkudesu.Kafka.Dotnet.Base;
using Rinkudesu.Kafka.Dotnet.Exceptions;

namespace Rinkudesu.Kafka.Dotnet;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

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
            await _producer.ProduceAsync(topic, message.GetMessage(), cancellationToken).ConfigureAwait(false);
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
    public virtual void ProduceBulk<T>(string topic, IEnumerable<T> messages, TimeSpan? waitTime = null) where T : GenericKafkaMessage
    {
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
            _producer.Flush(waitTime ?? TimeSpan.FromSeconds(10));
        }

        if (exceptions.Any()) throw new AggregateException(exceptions);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _producer.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
