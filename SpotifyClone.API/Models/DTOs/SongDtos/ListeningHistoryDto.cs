namespace SpotifyClone.API.Models.DTOs.SongDtos
{
    public class ListeningHistoryDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string AlbumCoverUrl { get; set; }
        public DateTime ListenedAt { get; set; }
    }
}
