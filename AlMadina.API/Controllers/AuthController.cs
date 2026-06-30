using Microsoft.AspNetCore.Mvc;
using AlMadina.Application.DTOs;
using AlMadina.Application.Interfaces.Services;

namespace AlMadina.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(new { token = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { message = "Invalid credentials" });
            return Ok(new { token = result });
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(
            SendOtpDto dto)
        {
            await _authService.SendOtpAsync(dto.Email);

            return Ok("OTP sent");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(
            VerifyOtpDto dto)
        {
            var result =
                await _authService.VerifyOtpAsync(dto);

            if (result == null)
                return BadRequest("Invalid OTP");

            return Ok(result);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(
            GoogleLoginDto dto)
        {
            var result =
                await _authService
                .GoogleLoginAsync(dto.IdToken);

            return Ok(result);
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> Profile(
            string id)
        {
            var user =
                await _authService
                .GetProfileAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            UpdateUserDto dto)
        {
            var result =
                await _authService
                .UpdateUserAsync(dto);

            if (!result)
                return NotFound();

            return Ok();
        }
    }
}