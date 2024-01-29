namespace core_messages.IntegrationEvents
{
    public class OrderPlacedEvent : IntegrationEventBase
    {
        public OrderPlacedEvent() : base()
        { 
        }

        public Guid OrderId { get; set; }
        public List<OrderItem> Items { get; set; }

        public class OrderItem
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
