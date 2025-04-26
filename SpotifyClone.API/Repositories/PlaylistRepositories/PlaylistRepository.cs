using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.PlaylistRepositories.PlaylistRepositoriesInterfaces;

namespace SpotifyClone.API.Repositories.PlaylistRepositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _context;

        public PlaylistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Playlist?> GetPlaylistByIdAsync(int id)
        {
            return await _context.Playlists.FindAsync(id);
        }

        public async Task<Playlist?> GetPlaylistWithSongsAsync(int id)
        {
            return await _context.Playlists.Include(p => p.PlaylistSongs).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPlaylistAsync(Playlist playlist)
        {
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePlaylistAsync(Playlist playlist)
        {
            _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsSongInPlaylistAsync(int playlistId, int songId)
        {
            return await _context.PlaylistSongs.AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
        }

        public async Task AddSongToPlaylistAsync(int playlistId, int songId)
        {
            _context.PlaylistSongs.Add(new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId
            });
            await _context.SaveChangesAsync();
        }

        public async Task<PlaylistSong?> GetPlaylistSongLinkAsync(int playlistId, int songId)
        {
            return await _context.PlaylistSongs.FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
        }

        public async Task RemoveSongFromPlaylistAsync(PlaylistSong link)
        {
            _context.PlaylistSongs.Remove(link);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Playlist>> SearchPlaylistsAsync(string query)
        {
            return await _context.Playlists
                .Where(p => p.Name.Contains(query))
                .ToListAsync();
        }
    }

}
