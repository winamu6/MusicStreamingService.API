namespace SpotifyClone.API.Models.DTOs
{
    public class UpdateProfileDto {
        public IFormFile? Avatar { get; set; }
        public string DisplayName { get; set; } 
        public string UserName { get; set; } 
        public string Email { get; set; } 
    }
}
