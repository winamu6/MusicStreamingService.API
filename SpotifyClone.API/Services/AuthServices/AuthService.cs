using Microsoft.Identity.Client.Extensions.Msal;
using SpotifyClone.API.Models;
using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.AuthRepositories.AuthRepositoriesInterfaces;
using SpotifyClone.API.Services.AuthServices.Interfaces;
using SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces;

namespace SpotifyClone.API.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly ISupabaseStorageService _storage;

        public AuthService(
            IUserRepository userRepository, 
            ITokenService tokenService, 
            IConfiguration config,
            ISupabaseStorageService supabaseStorageService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _config = config;
            _storage = supabaseStorageService;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterAsync(RegisterDto model)
        {
            string avatarPath = "default-avatar.png";

            if (model.Avatar != null)
            {
                var fileName = $"avatar_{Guid.NewGuid()}_{Path.GetFileName(model.Avatar.FileName)}";
                avatarPath = await _storage.UploadFileAsync("avatars", fileName, model.Avatar.OpenReadStream());
            }

            var user = new ApplicationUser
            {
                AvatarPath = avatarPath,
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

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateProfileAsync(string userId, UpdateProfileDto model)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
                return (false, "Пользователь не найден");

            user.DisplayName = model.DisplayName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            if (model.Avatar != null)
            {
                var fileName = $"avatar_{Guid.NewGuid()}_{Path.GetFileName(model.Avatar.FileName)}";
                var avatarPath = await _storage.UploadFileAsync("avatars", fileName, model.Avatar.OpenReadStream());
                user.AvatarPath = avatarPath;
            }
            else if (string.IsNullOrEmpty(user.AvatarPath))
            {
                user.AvatarPath = "default-avatar.png";
            }

            var result = await _userRepository.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            return (true, null);
        }



        public async Task<(bool IsSuccess, string ErrorMessage)> ChangePasswordAsync(string userId, ChangePasswordDto model)
        {
            var user = await _userRepository.FindByIdAsync(userId); if (user == null) return (false, "Пользователь не найден");

            var result = await _userRepository.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            return (true, null);

        }
    }

}
