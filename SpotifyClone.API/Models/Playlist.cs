namespace SpotifyClone.API.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}
