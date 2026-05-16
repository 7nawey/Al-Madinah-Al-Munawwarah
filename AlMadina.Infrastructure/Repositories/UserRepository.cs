using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AlMadina.Application.Interfaces;
using AlMadina.Domain.Entities;


namespace AlMadina.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetByPhoneAsync(string phone)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(x => x.PhoneNumber == phone);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task AddAsync(ApplicationUser user)
        {
            await _userManager.CreateAsync(user);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> ExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    }
}