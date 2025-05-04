namespace SpotifyClone.API.Models.DTOs
{
    public class AlbumUploadDto { 
        public string Title { get; set; } 
        public string ArtistName { get; set; } 
        public DateTime ReleaseDate { get; set; }
        public string GenreName { get; set; }
        public IFormFile? CoverImage { get; set; } 
    }
}
