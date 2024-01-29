using core_domain.Abstractions;

namespace domain.Entities
{
    public class Stock : IDocument
    { 
        public string Id { get; set; }

        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }
}
