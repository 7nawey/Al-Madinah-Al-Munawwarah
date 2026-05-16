using AlMadina.Application.DTOs;

namespace AlMadina.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(int pageNumber = 1, int pageSize = 20);

        Task<IEnumerable<ProductDto>> SearchAsync(string keyword);

        Task<ProductDto?> GetByIdAsync(Guid id);

        Task<ProductDto?> GetForUpdateAsync(string value);

        Task<IEnumerable<ProductDto>> GetFeaturedAsync();

        Task<IEnumerable<ProductDto>> GetDiscountedAsync();

        Task<IEnumerable<ProductDto>> GetBestSellingAsync();

        Task<IEnumerable<ProductDto>> GetOthersAsync();

        Task<PagedResult<ProductDto>> GetProductsByCategoryAsync(
            Guid categoryId,
            int pageNumber = 1,
            int pageSize = 40);

        Task<ProductDto> CreateAsync(CreateProductDto dto);

        Task<bool> UpdateAsync(UpdateProductDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}