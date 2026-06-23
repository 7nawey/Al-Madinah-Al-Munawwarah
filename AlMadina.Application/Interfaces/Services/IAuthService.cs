using AlMadina.Application.DTOs;

namespace AlMadina.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task SendOtpAsync(string email);

        Task<AuthResponseDto?> VerifyOtpAsync(VerifyOtpDto dto);

        Task<AuthResponseDto?> GoogleLoginAsync(string idToken);

        Task<UserDto?> GetProfileAsync(string userId);

        Task<bool> UpdateUserAsync(UpdateUserDto dto);
    }
}