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
        public string CoverImagePath { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; }

        public ICollection<Like> Likes { get; set; }
    }
}
