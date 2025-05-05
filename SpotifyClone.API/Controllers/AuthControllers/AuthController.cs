using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpotifyClone.API.Models;
using SpotifyClone.API.Models.DTOs.UserDtos;
using SpotifyClone.API.Services.AuthServices.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpotifyClone.API.Controllers.AuthControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto model)
        {
            var (isSuccess, errorMessage) = await _authService.RegisterAsync(model);

            if (!isSuccess)
                return BadRequest(new { error = errorMessage });

            return Ok("Пользователь зарегистрирован");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var (isSuccess, token, expiresIn, errorMessage) = await _authService.LoginAsync(model);

            if (!isSuccess)
                return Unauthorized(new { error = errorMessage });

            return Ok(new
            {
                token,
                expiresIn
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var (isSuccess, error) = await _authService.UpdateProfileAsync(userId, model);
            return isSuccess ? Ok("Данные обновлены") : BadRequest(new { error });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var (isSuccess, error) = await _authService.ChangePasswordAsync(userId, model);
            return isSuccess ? Ok("Пароль обновлён") : BadRequest(new { error });
        }
    }
}
