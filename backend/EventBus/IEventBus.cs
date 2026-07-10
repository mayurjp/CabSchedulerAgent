namespace CabScheduler.Api.EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(string topic, T @event);
    Task SubscribeAsync<T>(string topic, Func<T, Task> handler);
}
