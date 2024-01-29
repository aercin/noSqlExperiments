using core_domain.Entities;

namespace core_application.Abstractions
{
    public interface IInboxMessageRepository : IMongoRepository<InboxMessage>
    {
    }
}
