namespace domain.Entities
{
    public class Basket
    {
        public Basket()
        {
            Items = new List<BasketItem>();
        }

        public Guid UserId { get; set; }

        public List<BasketItem> Items { get; set; }
    }

    public class BasketItem
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
