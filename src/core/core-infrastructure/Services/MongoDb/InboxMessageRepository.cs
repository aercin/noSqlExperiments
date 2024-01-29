using core_application.Abstractions;
using core_domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace core_infrastructure.Services.MongoDb
{
    public class InboxMessageRepository : MongoRepository<InboxMessage>, IInboxMessageRepository
    {
        public InboxMessageRepository(IConfiguration configuration, IMongoClient client) : base(configuration.GetSection("MongoDb"), client)
        {
        }
    }
}
