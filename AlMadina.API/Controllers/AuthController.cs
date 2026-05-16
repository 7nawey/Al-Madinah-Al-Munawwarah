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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            var token = await _authService.LoginAsync(dto);

            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { token });
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _authService.GetProfileAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateUserDto dto)
        {
            var result = await _authService.UpdateUserAsync(dto);

            if (!result)
                return NotFound();

            return Ok("Updated successfully");
        }
    }
}