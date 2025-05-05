using SpotifyClone.API.Models.Entities;

namespace SpotifyClone.API.Models.DTOs.UserDtos
{
    public class RegisterDto
    {
        public IFormFile? Avatar { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
