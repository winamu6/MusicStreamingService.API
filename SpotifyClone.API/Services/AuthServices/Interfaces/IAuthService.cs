using SpotifyClone.API.Models.DTOs.UserDtos;

namespace SpotifyClone.API.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string ErrorMessage)> RegisterAsync(RegisterDto model);
        Task<(bool IsSuccess, string Token, DateTime ExpiresIn, string ErrorMessage)> LoginAsync(LoginDto model);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateProfileAsync(string userId, UpdateProfileDto model); 
        Task<(bool IsSuccess, string ErrorMessage)> ChangePasswordAsync(string userId, ChangePasswordDto model);
    }

}
