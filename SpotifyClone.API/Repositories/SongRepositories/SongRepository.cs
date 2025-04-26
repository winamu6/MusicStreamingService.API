using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.Models.Common;
using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.SongRepositories.SongRepositoriesInterfaces;

namespace SpotifyClone.API.Repositories.SongRepositories
{
    public class SongRepository : ISongRepository
    {
        private readonly ApplicationDbContext _context;

        public SongRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AlbumExistsAsync(int albumId)
        {
            return await _context.Albums.AnyAsync(a => a.Id == albumId);
        }

        public async Task AddSongAsync(Song song)
        {
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
        }

        public async Task<Song?> GetSongByIdAsync(int id)
        {
            return await _context.Songs.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateSongAsync(Song song)
        {
            _context.Songs.Update(song);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSongAsync(Song song)
        {
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Song>> SearchSongsAsync(string? query, int page, int pageSize, string? sortBy, bool descending)
        {
            var songsQuery = _context.Songs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var words = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    songsQuery = songsQuery.Where(s =>
                        EF.Functions.Like(s.Title.ToLower(), $"%{word}%") ||
                        EF.Functions.Like(s.ArtistName.ToLower(), $"%{word}%")
                    );
                }
            }

            songsQuery = sortBy?.ToLower() switch
            {
                "author" => descending ? songsQuery.OrderByDescending(s => s.ArtistName) : songsQuery.OrderBy(s => s.ArtistName),
                _ => descending ? songsQuery.OrderByDescending(s => s.Title) : songsQuery.OrderBy(s => s.Title)
            };

            var total = await songsQuery.CountAsync();
            var items = await songsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Song>(items, total, page, pageSize);
        }

        public async Task AddListeningHistoryAsync(ListeningHistory history)
        {
            _context.ListeningHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ListeningHistoryDto>> GetListeningHistoryAsync(string userId)
        {
            return await _context.ListeningHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ListenedAt)
                .Include(h => h.Song)
                .Select(h => new ListeningHistoryDto
                {
                    SongId = h.Song.Id,
                    Title = h.Song.Title,
                    ArtistName = h.Song.ArtistName,
                    Genre = h.Song.Genre,
                    Duration = h.Song.Duration,
                    AudioFilePath = h.Song.AudioFilePath,
                    ListenedAt = h.ListenedAt
                })
                .ToListAsync();
        }

    }

}
