using System.ComponentModel.DataAnnotations;

namespace SpotifyClone.API.Models.DTOs
{
    public class SongUploadDto
    {
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public int Duration { get; set; }
        public int AlbumId { get; set; }
        public IFormFile AudioFile { get; set; }

    }

}
