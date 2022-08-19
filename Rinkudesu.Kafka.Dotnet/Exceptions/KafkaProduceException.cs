using System.Diagnostics.CodeAnalysis;
using Rinkudesu.Kafka.Dotnet.Base;

namespace Rinkudesu.Kafka.Dotnet.Exceptions;

[ExcludeFromCodeCoverage]
public class KafkaProduceException : KafkaException
{
    public Confluent.Kafka.ProduceException<Confluent.Kafka.Null, string>? KafkaException { get; }

    public KafkaProduceException(string message) : base(message)
    {
    }

    public KafkaProduceException(Confluent.Kafka.ProduceException<Confluent.Kafka.Null, string> exception) : base(exception.Message, exception)
    {
        KafkaException = exception;
    }
}
