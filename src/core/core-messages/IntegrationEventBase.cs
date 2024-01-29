namespace core_messages
{
    public class IntegrationEventBase
    {
        public IntegrationEventBase()
        {
            EventType = this.GetType().FullName;
            MessageId = Guid.NewGuid();
        }

        public string EventType { get; set; }
        public Guid MessageId { get; set; }
    }
}
