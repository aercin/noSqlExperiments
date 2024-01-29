using core_application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace core_infrastructure.Services.MongoDb
{
    public abstract class BaseUnitOfWork : IBaseUnitOfWork
    {
        private IClientSessionHandle _session;
        private IServiceProvider _serviceProvider;

        public BaseUnitOfWork(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task<IDisposable> StartNewSessionAsync()
        {
            this._session = await this._serviceProvider.GetService<IMongoClient>().StartSessionAsync();
            this._session.StartTransaction();
            return this._session;
        }

        public async Task CommitAsync()
        {
            await this._session.CommitTransactionAsync();
            this._session.Dispose();
        }

        public async Task RollbackAsync()
        {
            await this._session.AbortTransactionAsync();
            this._session.Dispose();
        }

        public IInboxMessageRepository InboxMessageRepo
        {
            get
            {
                return this._serviceProvider.GetService<IInboxMessageRepository>();
            }
        }

        public IOutboxMessageRepository OutboxMessageRepo
        {
            get
            {
                return this._serviceProvider.GetService<IOutboxMessageRepository>();
            }
        }
    }
}
