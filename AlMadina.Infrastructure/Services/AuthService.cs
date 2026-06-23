using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlMadina.Application.DTOs;
using AlMadina.Application.Interfaces.Services;
using AlMadina.Domain.Entities;

namespace AlMadina.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IConfiguration config,
            IMemoryCache cache,
            IEmailService emailService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
            _cache = cache;
            _emailService = emailService;
        }

        // ================= REGISTER =================

        public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
        {
            var exists = await _userManager.FindByEmailAsync(dto.Email);

            if (exists != null)
                throw new Exception("Email already exists");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception("User creation failed");

            return _mapper.Map<UserDto>(user);
        }

        // ================= LOGIN =================

        public async Task<string?> LoginAsync(LoginUserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return null;

            var check = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!check)
                return null;

            return GenerateToken(user);
        }

        // ================= SEND OTP =================

        public async Task SendOtpAsync(string email)
        {
            var otp = GenerateOtp();

            _cache.Set(
                email,
                otp,
                TimeSpan.FromMinutes(5));

            await _emailService.SendOtpAsync(email, otp);
        }

        // ================= VERIFY OTP =================

        public async Task<AuthResponseDto?> VerifyOtpAsync(
            VerifyOtpDto dto)
        {
            if (!_cache.TryGetValue(dto.Email, out string? storedOtp))
                return null;

            if (storedOtp != dto.Otp)
                return null;

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.Email.Split('@')[0],
                    Address = "",
                    IsActive = true
                };

                var createResult =
                    await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                    return null;
            }

            _cache.Remove(dto.Email);

            var token = GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = _mapper.Map<UserDto>(user)
            };
        }

        // ================= GOOGLE LOGIN =================

        public async Task<AuthResponseDto?> GoogleLoginAsync(
            string idToken)
        {
            var payload =
                await GoogleJsonWebSignature
                    .ValidateAsync(idToken);

            var user =
                await _userManager
                    .FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FullName = payload.Name,
                    Address = "",
                    IsActive = true
                };

                var createResult =
                    await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                    return null;
            }

            var token = GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = _mapper.Map<UserDto>(user)
            };
        }

        // ================= PROFILE =================

        public async Task<UserDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user == null
                ? null
                : _mapper.Map<UserDto>(user);
        }

        // ================= UPDATE =================

        public async Task<bool> UpdateUserAsync(UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);

            if (user == null)
                return false;

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.Address = dto.Address;
            user.PhoneNumber = dto.PhoneNumber;
            user.IsActive = dto.IsActive;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        // ================= OTP =================

        private string GenerateOtp()
        {
            return new Random()
                .Next(100000, 999999)
                .ToString();
        }

        // ================= JWT =================

        private string GenerateToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("FullName", user.FullName ?? "")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}