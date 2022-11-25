using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
#pragma warning disable CS1591

namespace Rinkudesu.Kafka.Dotnet.Base;

[Serializable]
[ExcludeFromCodeCoverage]
[SuppressMessage("Documentation", "CS1591")]
public abstract class KafkaException : Exception
{
    protected KafkaException()
    {
    }

    protected KafkaException(string message) : base(message)
    {
    }

    protected KafkaException(string message, Exception inner) : base(message, inner)
    {
    }

    protected KafkaException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
