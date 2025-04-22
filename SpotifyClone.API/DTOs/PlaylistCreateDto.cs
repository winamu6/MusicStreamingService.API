namespace SpotifyClone.API.DTOs
{
    public class PlaylistCreateDto
    {
        public string Name { get; set; }
        public IFormFile? CoverImage { get; set; }
    }
}
