namespace CabScheduler.Api.EventBus;

public class StubEventBus : IEventBus
{
    private readonly ILogger<StubEventBus> _logger;
    private readonly Dictionary<string, List<Delegate>> _subscriptions = new();

    public StubEventBus(ILogger<StubEventBus> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(string topic, T @event)
    {
        _logger.LogInformation("[EventBus] Published to '{Topic}': {@Event}", topic, @event);

        if (_subscriptions.TryGetValue(topic, out var handlers))
        {
            foreach (var handler in handlers)
            {
                if (handler is Func<T, Task> typedHandler)
                {
                    _ = typedHandler(@event);
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(string topic, Func<T, Task> handler)
    {
        _logger.LogInformation("[EventBus] Subscribed to '{Topic}'", topic);

        if (!_subscriptions.ContainsKey(topic))
            _subscriptions[topic] = new List<Delegate>();

        _subscriptions[topic].Add(handler);

        return Task.CompletedTask;
    }
}
