using SpotifyClone.API.Models.Entities;

namespace SpotifyClone.API.Services.AuthServices.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
    }
}
