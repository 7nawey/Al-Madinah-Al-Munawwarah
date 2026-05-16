using AlMadina.Domain.Entities;

namespace AlMadina.Application.Interfaces
{
    public interface IProductRepository
        : IGenericRepository<Product>
    {
        IQueryable<Product> GetAllQueryable();

        Task<int> CountAsync();

        Task<IEnumerable<Product>> GetFeaturedAsync();

        Task<IEnumerable<Product>> GetDiscountedAsync();

        Task<IEnumerable<Product>> GetBestSellingAsync();

        Task<IEnumerable<Product>> GetOthersAsync();
    }
}