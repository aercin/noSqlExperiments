namespace api.Models
{
    public class AddProductToStockRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
