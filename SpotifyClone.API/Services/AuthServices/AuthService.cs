using SpotifyClone.API.Models;
using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.AuthRepositories.AuthRepositoriesInterfaces;
using SpotifyClone.API.Services.AuthServices.Interfaces;

namespace SpotifyClone.API.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IConfiguration config)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _config = config;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterAsync(RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName,
                Role = model.Role
            };

            var result = await _userRepository.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userRepository.AddToRoleAsync(user, model.Role.ToString());

            return (true, null);
        }

        public async Task<(bool IsSuccess, string Token, DateTime ExpiresIn, string ErrorMessage)> LoginAsync(LoginDto model)
        {
            var user = await _userRepository.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, null, default, "Неверный логин или пароль");

            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return (false, null, default, "Неверный логин или пароль");

            var roles = await _userRepository.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            var jwtSettings = _config.GetSection("Jwt");
            var expireMinutes = double.TryParse(jwtSettings["ExpireMinutes"], out var minutes) ? minutes : 60;
            var expiresIn = DateTime.UtcNow.AddMinutes(expireMinutes);

            return (true, token, expiresIn, null);
        }
    }

}
