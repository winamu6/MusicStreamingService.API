using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.AlbumRepositories.AlbumRepositoriesInterfaces;

namespace SpotifyClone.API.Repositories.AlbumRepositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly ApplicationDbContext _context;

        public AlbumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Album?> GetAlbumByIdAsync(int id)
        {
            return await _context.Albums.FindAsync(id);
        }

        public async Task<Album?> GetAlbumWithSongsAsync(int id)
        {
            return await _context.Albums.Include(a => a.Songs).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Album>> GetAllAlbumsAsync()
        {
            return await _context.Albums.Include(a => a.Songs).ToListAsync();
        }

        public async Task AddAlbumAsync(Album album)
        {
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAlbumAsync(Album album)
        {
            _context.Albums.Update(album);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAlbumAsync(Album album)
        {
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
        }

        public async Task<object> SearchAlbumsAsync(string? query, int page, int pageSize, string? sortBy, bool descending)
        {
            var albumsQuery = _context.Albums.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var words = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    albumsQuery = albumsQuery.Where(a =>
                        EF.Functions.Like(a.Title.ToLower(), $"%{word}%") ||
                        EF.Functions.Like(a.ArtistName.ToLower(), $"%{word}%")
                    );
                }
            }

            albumsQuery = sortBy?.ToLower() switch
            {
                "author" => descending ? albumsQuery.OrderByDescending(a => a.ArtistName) : albumsQuery.OrderBy(a => a.ArtistName),
                _ => descending ? albumsQuery.OrderByDescending(a => a.Title) : albumsQuery.OrderBy(a => a.Title)
            };

            var total = await albumsQuery.CountAsync();
            var albums = await albumsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new
            {
                TotalItems = total,
                Page = page,
                PageSize = pageSize,
                Items = albums
            };
        }
    }

}
