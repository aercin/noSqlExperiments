using core_domain.Abstractions;
using domain.Enumerations;

namespace domain.Entities
{
    public class Order : IDocument
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public List<Product> Products { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
