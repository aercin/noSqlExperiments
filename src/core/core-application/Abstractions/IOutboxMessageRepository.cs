using core_domain.Entities;

namespace core_application.Abstractions
{
    public interface IOutboxMessageRepository : IMongoRepository<OutboxMessage>
    { 
    }
}
