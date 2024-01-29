using application.Abstractions;
using core_infrastructure.Services.MongoDb;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure.Services
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        private IServiceProvider _serviceProvider;
        public UnitOfWork(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IStockRepository StockRepo
        {
            get
            {
                return this._serviceProvider.GetService<IStockRepository>();
            }
        }
    }
}
