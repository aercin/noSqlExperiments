using core_application.Abstractions;
using domain.Entities;

namespace application.Abstractions
{
    public interface IOrderRepository : IMongoRepository<Order>
    {
    }
}
