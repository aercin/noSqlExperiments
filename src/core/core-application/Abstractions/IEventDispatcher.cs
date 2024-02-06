namespace core_application.Abstractions
{
    public interface IEventDispatcher
    {
        Task DispatchEventAsync(object message);
    }
}
