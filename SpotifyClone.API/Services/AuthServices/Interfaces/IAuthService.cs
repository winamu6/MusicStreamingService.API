using SpotifyClone.API.Models.DTOs;

namespace SpotifyClone.API.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string ErrorMessage)> RegisterAsync(RegisterDto model);
        Task<(bool IsSuccess, string Token, DateTime ExpiresIn, string ErrorMessage)> LoginAsync(LoginDto model);
    }

}
