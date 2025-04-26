namespace SpotifyClone.API.Models.DTOs
{
    public class ListeningHistoryDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public TimeSpan Duration { get; set; }
        public string AudioFilePath { get; set; }
        public DateTime ListenedAt { get; set; }
    }
}
