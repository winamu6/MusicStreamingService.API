namespace SpotifyClone.API.Models.Entities
{
    public class ListeningHistory
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        public DateTime ListenedAt { get; set; }
    }
}
