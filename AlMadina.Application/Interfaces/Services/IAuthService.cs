using AlMadina.Application.DTOs;

namespace AlMadina.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterUserDto dto);

        Task<string?> LoginAsync(LoginUserDto dto);

        Task<UserDto?> GetProfileAsync(string userId);

        Task<bool> UpdateUserAsync(UpdateUserDto dto);
    }
}