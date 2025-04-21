using SpotifyClone.API.Models;

namespace SpotifyClone.API.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } // Enum
    }
}
