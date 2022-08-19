using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Rinkudesu.Kafka.Dotnet.Base;

namespace Rinkudesu.Kafka.Dotnet.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class KafkaProduceException : KafkaException
{
    public Confluent.Kafka.ProduceException<Confluent.Kafka.Null, string>? KafkaException { get; }

    public KafkaProduceException()
    {
    }

    public KafkaProduceException(string message) : base(message)
    {
    }

    public KafkaProduceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public KafkaProduceException(Confluent.Kafka.ProduceException<Confluent.Kafka.Null, string> exception) : base(exception.Message, exception)
    {
        KafkaException = exception;
    }

    protected KafkaProduceException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
