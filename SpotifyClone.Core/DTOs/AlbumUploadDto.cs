namespace SpotifyClone.API.DTOs
{
    public class AlbumUploadDto { 
        public string Title { get; set; } 
        public string ArtistName { get; set; } 
        public DateTime ReleaseDate { get; set; } 
        public IFormFile? CoverImage { get; set; } 
    }
}
