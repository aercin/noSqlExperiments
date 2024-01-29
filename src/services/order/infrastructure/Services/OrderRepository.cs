using application.Abstractions;
using core_infrastructure.Services.MongoDb;
using domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace infrastructure.Services
{
    public class OrderRepository : MongoRepository<Order>, IOrderRepository
    {
        public OrderRepository(IConfiguration configuration, IMongoClient client) : base(configuration.GetSection("MongoDb"), client)
        {
        }
    }
}
