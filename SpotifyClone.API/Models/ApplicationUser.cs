using Microsoft.AspNetCore.Identity;

namespace SpotifyClone.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public UserRole Role { get; set; }

        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<Like> Likes { get; set; }
    }

    public enum UserRole
    {
        Listener,
        Musician,
        Admin
    }

}
