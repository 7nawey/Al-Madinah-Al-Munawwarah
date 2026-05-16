using AlMadina.Application.DTOs;
using Microsoft.AspNetCore.Http;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<bool> UpdateAsync(UpdateCategoryDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public interface IFileService
{
    Task<string?> UploadImageAsync(IFormFile file);
}