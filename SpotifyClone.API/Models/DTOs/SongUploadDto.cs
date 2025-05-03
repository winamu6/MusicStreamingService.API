using System.ComponentModel.DataAnnotations;

namespace SpotifyClone.API.Models.DTOs
{
    public class SongUploadDto
    {
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string GenreName { get; set; }
        public int Duration { get; set; }
        public int AlbumId { get; set; }
        public IFormFile AudioFile { get; set; }
        public float Tempo { get; set; }
        public float Energy { get; set; }
        public float Danceability { get; set; }
    }
}
