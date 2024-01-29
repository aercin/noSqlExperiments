namespace core_application.Abstractions
{
    public interface IEventListener
    {
        Task ConsumeEvent(string topic, Func<string, Task> callback);
    }
}
