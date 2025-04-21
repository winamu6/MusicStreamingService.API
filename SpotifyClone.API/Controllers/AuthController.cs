using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpotifyClone.API.DTOs;
using SpotifyClone.API.Models;
using SpotifyClone.API.Services.AuthServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpotifyClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = config;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName,
                Role = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Назначить роль
            await _userManager.AddToRoleAsync(user, model.Role.ToString());

            return Ok("Пользователь зарегистрирован");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Неверный логин или пароль");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Неверный логин или пароль");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            // читаем срок действия из конфигурации
            var jwtSettings = _config.GetSection("Jwt");
            var expireMinutes = double.TryParse(jwtSettings["ExpireMinutes"], out var minutes) ? minutes : 60;
            var expiresIn = DateTime.UtcNow.AddMinutes(expireMinutes);

            return Ok(new
            {
                token,
                expiresIn
            });
        }
    }
}
