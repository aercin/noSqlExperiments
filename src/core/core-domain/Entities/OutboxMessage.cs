using core_domain.Abstractions;

namespace core_domain.Entities
{
    public class OutboxMessage : IDocument
    {
        public string Id { get; set; }
        public string ServiceName { get; set; }
        public string Message { get; set; }
        public string Topic { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
