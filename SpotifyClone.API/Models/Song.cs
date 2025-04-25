namespace SpotifyClone.API.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public TimeSpan Duration { get; set; }

        public string AudioFilePath { get; set; }

        public int ListenCount { get; set; } = 0;

        public int AlbumId { get; set; }

        public ICollection<Like> Likes { get; set; }
    }
}
