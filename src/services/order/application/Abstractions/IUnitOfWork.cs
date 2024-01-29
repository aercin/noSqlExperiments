using core_application.Abstractions;

namespace application.Abstractions
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IOrderRepository OrderRepo { get; }
    }
}
