using AlMadina.Domain.Entities;

namespace AlMadina.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);

        Task<ApplicationUser?> GetByEmailAsync(string email);

        Task<ApplicationUser?> GetByPhoneAsync(string phone);

        Task<IEnumerable<ApplicationUser>> GetAllAsync();

        Task AddAsync(ApplicationUser user);

        Task UpdateAsync(ApplicationUser user);

        Task<bool> ExistsAsync(string email);
    }
}