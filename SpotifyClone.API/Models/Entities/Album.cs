namespace SpotifyClone.API.Models.Entities
{
    public class Album
    {
        public int Id { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string CoverImagePath { get; set; }

        public ICollection<Song> Songs { get; set; }
    }
}
