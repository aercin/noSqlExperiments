namespace application.Features.Queries.Models
{
    public class ProductQueryOptions
    {
        public int CategoryId { get; set; }
        public string Code { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100; 
    }
}
