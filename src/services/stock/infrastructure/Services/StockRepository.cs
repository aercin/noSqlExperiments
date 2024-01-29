using application.Abstractions;
using core_infrastructure.Services.MongoDb;
using domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace infrastructure.Services
{
    public class StockRepository : MongoRepository<Stock>, IStockRepository
    {
        public StockRepository(IConfiguration config, IMongoClient client) : base(config.GetSection("MongoDb"), client)
        {
        }
    }
}
