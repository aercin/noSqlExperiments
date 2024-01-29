namespace core_application.Abstractions
{
    public interface IBaseUnitOfWork
    {
        Task<IDisposable> StartNewSessionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        IOutboxMessageRepository OutboxMessageRepo { get; }
        IInboxMessageRepository InboxMessageRepo { get; }
    }
}
