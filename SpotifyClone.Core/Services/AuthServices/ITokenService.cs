using SpotifyClone.API.Models;

namespace SpotifyClone.API.Services.AuthServices
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
    }
}
