namespace core_messages.IntegrationEvents
{
    public class StockNotDecreasedEvent : IntegrationEventBase
    {
        public StockNotDecreasedEvent() : base()
        {

        }
        public Guid OrderId { get; set; }
    }
}
