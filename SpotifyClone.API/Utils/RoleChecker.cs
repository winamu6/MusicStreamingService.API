using System.Security.Claims;

namespace SpotifyClone.API.Utils
{
    public static class RoleChecker
    {
        public static bool IsMusicianOrAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole("Musician") || user.IsInRole("Admin");
        }
    }

}
