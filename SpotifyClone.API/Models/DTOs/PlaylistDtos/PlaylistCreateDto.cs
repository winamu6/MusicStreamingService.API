namespace SpotifyClone.API.Models.DTOs.PlaylistDtos
{
    public class PlaylistCreateDto
    {
        public string Name { get; set; }
        public IFormFile? CoverImage { get; set; }
    }
}
