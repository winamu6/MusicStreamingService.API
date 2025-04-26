using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpotifyClone.API.DTOs;
using SpotifyClone.API.Models;
using SpotifyClone.API.Services.AuthServices.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpotifyClone.API.Controllers
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
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
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
    }

}
