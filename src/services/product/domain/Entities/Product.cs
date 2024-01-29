namespace domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public int CategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Cas { get; set; }
    }
}

