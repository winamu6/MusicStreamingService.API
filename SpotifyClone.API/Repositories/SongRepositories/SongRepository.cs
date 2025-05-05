using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using SpotifyClone.API.Data;
using SpotifyClone.API.Models.Common;
using SpotifyClone.API.Models.DTOs.SongDtos;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.SongRepositories.SongRepositoriesInterfaces;
using SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces;
using Supabase.Interfaces;

namespace SpotifyClone.API.Repositories.SongRepositories
{
    public class SongRepository : ISongRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ISupabaseStorageService _supabaseStorageService;

        public SongRepository(
            ApplicationDbContext context,
            ISupabaseStorageService supabaseStorageService)
        {
            _context = context;
            _supabaseStorageService = supabaseStorageService;
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

        public async Task<PagedResult<SongDto>> SearchSongsAsync(
            string? query, int page, int pageSize, string? sortBy, bool descending)
        {
            var songsQuery = _context.Songs
                                .Include(s => s.Album)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var words = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    songsQuery = songsQuery.Where(s =>
                        EF.Functions.Like(s.Title.ToLower(), $"%{word}%") ||
                        EF.Functions.Like(s.ArtistName.ToLower(), $"%{word}%"));
                }
            }

            songsQuery = sortBy?.ToLower() switch
            {
                "author" => descending ? songsQuery.OrderByDescending(s => s.ArtistName) : songsQuery.OrderBy(s => s.ArtistName),
                _ => descending ? songsQuery.OrderByDescending(s => s.Title) : songsQuery.OrderBy(s => s.Title)
            };

            var total = await songsQuery.CountAsync();

            var items = await songsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var results = new List<SongDto>();

            foreach (var song in items)
            {
                var coverUrl = await _supabaseStorageService.GetPublicUrlAsync("albums", song.Album.CoverImagePath);

                results.Add(new SongDto
                {
                    Id = song.Id,
                    Title = song.Title,
                    ArtistName = song.ArtistName,
                    AlbumCoverUrl = coverUrl
                });
            }

            return new PagedResult<SongDto>(results, total, page, pageSize);
        }

        public async Task AddListeningHistoryAsync(ListeningHistory history)
        {
            _context.ListeningHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ListeningHistoryDto>> GetListeningHistoryAsync(string userId)
        {
            var histories = await _context.ListeningHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ListenedAt)
                .Include(h => h.Song)
                    .ThenInclude(s => s.Album)
                .Include(h => h.Song.Genre)
                .ToListAsync();

            var result = new List<ListeningHistoryDto>();

            foreach (var h in histories)
            {
                var albumCoverUrl = await _supabaseStorageService.GetPublicUrlAsync("albums", h.Song.Album.CoverImagePath);

                result.Add(new ListeningHistoryDto
                {
                    SongId = h.Song.Id,
                    Title = h.Song.Title,
                    ArtistName = h.Song.ArtistName,
                    ListenedAt = h.ListenedAt,
                    AlbumCoverUrl = albumCoverUrl
                });
            }

            return result;
        }
        public async Task<List<Song>> GetContentBasedRecommendationsAsync(Song referenceSong, int maxResults = 100)
        {
            return await _context.Songs
                .Where(s => s.Id != referenceSong.Id)
                .OrderBy(s =>
                    Math.Abs(s.Tempo - referenceSong.Tempo) +
                    Math.Abs(s.Energy - referenceSong.Energy) +
                    Math.Abs(s.Danceability - referenceSong.Danceability) +
                    (s.GenreId != referenceSong.GenreId ? 1 : 0))
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<List<int>> GetUserListenedSongIdsAsync(string userId)
        {
            return await _context.ListeningHistories
                .Where(h => h.UserId == userId)
                .Select(h => h.SongId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<SongFeatureDto>> GetRecentSongFeaturesAsync(string userId, int count)
        {
            return await _context.ListeningHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ListenedAt)
                .Select(h => new SongFeatureDto
                {
                    Tempo = h.Song.Tempo,
                    Energy = h.Song.Energy,
                    Danceability = h.Song.Danceability
                })
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Song>> GetSimilarSongsAsync(float tempo, float energy, float danceability, List<int> excludeSongIds, int limit)
        {
            return await _context.Songs
                .Where(s => !excludeSongIds.Contains(s.Id))
                .Select(s => new
                {
                    Song = s,
                    Distance = Math.Sqrt(
                        Math.Pow(s.Tempo - tempo, 2) +
                        Math.Pow(s.Energy - energy, 2) +
                        Math.Pow(s.Danceability - danceability, 2)
                    )
                })
                .OrderBy(x => x.Distance)
                .Take(limit)
                .Select(x => x.Song)
                .ToListAsync();
        }

    }
}
