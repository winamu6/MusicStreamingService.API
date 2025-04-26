using SpotifyClone.API.Models;

namespace SpotifyClone.API.Services.AuthServices.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
    }
}
