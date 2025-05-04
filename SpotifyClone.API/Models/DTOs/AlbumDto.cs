namespace SpotifyClone.API.Models.DTOs
{
    public class AlbumDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string ArtistName { get; set; } = default!;
        public DateTime ReleaseDate { get; set; }
        public string? CoverImageUrl { get; set; }
        public string GenreName { get; set; } = default!;
        public List<SongDto> Songs { get; set; } = new();
    }

    public class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
    }
}
