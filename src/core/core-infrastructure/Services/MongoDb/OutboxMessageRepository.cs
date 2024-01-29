using core_application.Abstractions;
using core_domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace core_infrastructure.Services.MongoDb
{
    public class OutboxMessageRepository : MongoRepository<OutboxMessage>, IOutboxMessageRepository
    {
        public OutboxMessageRepository(IConfiguration configuration, IMongoClient client) : base(configuration.GetSection("MongoDb"), client)
        {
        }
    }
}
