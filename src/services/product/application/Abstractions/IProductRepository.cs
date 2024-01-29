using application.Features.Queries.Models;
using domain.Entities;

namespace application.Abstractions
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);

        Task<bool> UpdateAsync(Product product);

        Task<bool> DeleteAsync(Guid productId, string cas);

        Task<Product?> GetAsync(Guid productId);
        Task<List<Product>> GetAsync(Action<ProductQueryOptions> options);
    }
}
