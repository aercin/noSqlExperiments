namespace core_application.Abstractions
{
    public interface ICorrelationService
    {
        string CorrelationId { get; }
    }
}
