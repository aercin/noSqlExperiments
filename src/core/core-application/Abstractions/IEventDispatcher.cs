namespace core_application.Abstractions
{
    public interface IEventDispatcher
    {
        Task DispatchEventAsync(string topic, string message);
    }
}
