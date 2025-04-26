using SpotifyClone.API.Models;

namespace SpotifyClone.API.Repositories.PlaylistRepositories.PlaylistRepositoriesInterfaces
{
    public interface IPlaylistRepository
    {
        Task<Playlist?> GetPlaylistByIdAsync(int id);
        Task<Playlist?> GetPlaylistWithSongsAsync(int id);
        Task AddPlaylistAsync(Playlist playlist);
        Task DeletePlaylistAsync(Playlist playlist);
        Task<bool> IsSongInPlaylistAsync(int playlistId, int songId);
        Task AddSongToPlaylistAsync(int playlistId, int songId);
        Task<PlaylistSong?> GetPlaylistSongLinkAsync(int playlistId, int songId);
        Task RemoveSongFromPlaylistAsync(PlaylistSong link);
        Task<List<Playlist>> SearchPlaylistsAsync(string query);
    }

}
