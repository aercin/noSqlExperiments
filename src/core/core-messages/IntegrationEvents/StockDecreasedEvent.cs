namespace core_messages.IntegrationEvents
{
    public class StockDecreasedEvent : IntegrationEventBase
    {
        public StockDecreasedEvent() : base()
        {

        }

        public Guid OrderId { get; set; }
    }
}
