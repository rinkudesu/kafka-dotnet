using System.Globalization;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rinkudesu.Kafka.Dotnet.Base;

namespace Rinkudesu.Kafka.Dotnet;

public class KafkaSubscriber<T> : IKafkaSubscriber<T> where T : GenericKafkaMessage
{
#pragma warning disable CA2213 // this is disposed, but for some reason dotnet warnings are still issued
    private readonly IConsumer<Null, string> _consumer;
#pragma warning restore CA2213
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaSubscriber<T>> _logger;

    private IKafkaSubscriberHandler<T>? handler;

    private Task? handleTask;
#pragma warning disable CA2213 //same story as above
    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationTokenSource? _combinedTaskCancellation;
#pragma warning restore CA2213

    public KafkaSubscriber(KafkaConfigurationProvider kafkaConfig, IServiceScopeFactory scopeFactory, ILogger<KafkaSubscriber<T>> logger)
    {
        _consumer = new ConsumerBuilder<Null, string>(kafkaConfig.GetConsumerConfig()).Build();
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    //todo: make sure this starts reading at a good place after reset
    public void Subscribe(IKafkaSubscriberHandler<T> newHandler)
    {
        _logger.LogDebug("Trying to subscribe to topic {Topic}", newHandler.Topic);
        handler = newHandler;
        _consumer.Subscribe(newHandler.Topic);
        _logger.LogInformation("Subscribed to topic {Topic}", newHandler.Topic);
    }

    public async Task<bool> Unsubscribe()
    {
        if (handler is null) return false;

        _logger.LogDebug("Trying to unsubscribe from {Topic}", handler.Topic);
        await StopHandle().ConfigureAwait(false);
        _consumer.Unsubscribe();
        _logger.LogInformation("Unsubscribed from {Topic}", handler.Topic);
        handler = null;
        return true;
    }

    public void BeginHandle(CancellationToken cancellationToken)
    {
        if (handler is null) throw new InvalidOperationException("Handler has not yet been registered");

        _cancellationTokenSource = new();
        _combinedTaskCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

        _logger.LogInformation("Beginning to handle topic {Topic}", handler.Topic);
        handleTask = Task.Run(() => Consume(_combinedTaskCancellation.Token), cancellationToken);
    }

    public async Task StopHandle()
    {
        if (handleTask is null || handleTask.IsCompleted) return;

        _cancellationTokenSource!.Cancel();
        try
        {
            await handleTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected as cancellation is the only way for handle task to exit
        }
#pragma warning disable CA1031 // this is kind-of a last chance handler in this case
        catch (Exception e)
#pragma warning restore CA1031
        {
            _logger.LogWarning(e, "Unexpected exception caught during subscriber shutdown.");
        }
        _cancellationTokenSource = null;
        handleTask = null;
    }

    protected virtual async Task Consume(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var handleResult = false;
            ConsumeResult<Null, string>? consumeResult = null;

            using var scope = _scopeFactory.CreateScope();
            try
            {
                consumeResult = _consumer.Consume(cancellationToken);
                var message = GenericKafkaMessage.Deserialise<T>(consumeResult.Message.Value) ?? throw new InvalidOperationException($"Failed to parse {consumeResult.Message.Value} as message");
                handleResult = await handler!.SetScope(scope).Handle(message, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consume is being canceled.");
                throw;
            }
#pragma warning disable CA1031 // this is kind-of a last chance handler in this case
            catch (Exception e)
#pragma warning restore CA1031
            {
                _logger.LogWarning(e, "Failure during consumption, message offset: {Offset}", consumeResult?.Offset.Value.ToString(CultureInfo.InvariantCulture));
            }
            finally
            {
                if (consumeResult is not null && handleResult)
                {
                    _consumer.Commit(consumeResult);
                }
            }
        }
        cancellationToken.ThrowIfCancellationRequested();
    }

    protected virtual async Task DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            await Unsubscribe().ConfigureAwait(false);
            _consumer.Close();
            _consumer.Dispose();
            _cancellationTokenSource?.Dispose();
            _combinedTaskCancellation?.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
