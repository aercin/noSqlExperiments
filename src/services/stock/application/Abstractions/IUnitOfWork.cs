using core_application.Abstractions;

namespace application.Abstractions
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IStockRepository StockRepo { get; }
    }
}
