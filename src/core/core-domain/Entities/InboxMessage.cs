using core_domain.Abstractions;

namespace core_domain.Entities
{
    public class InboxMessage : IDocument
    {
        public string Id { get; set; }
        public string ConsumerType { get; set; }
        public string MessageId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
