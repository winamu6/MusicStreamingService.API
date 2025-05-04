using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.GenreRepositories.GenreRepositoriesInterfaces;

namespace SpotifyClone.API.Repositories.GenreRepositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> GetOrCreateGenreAsync(string genreName)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == genreName.ToLower());
            if (genre == null)
            {
                genre = new Genre { Name = genreName };
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
            }

            return genre;
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<List<Song>> GetTopSongsByGenreAsync(int genreId, int limit)
        {
            return await _context.Songs
                .Where(s => s.GenreId == genreId)
                .OrderByDescending(s => s.ListenCount)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Album>> GetAlbumsByGenreAsync(int genreId, int limit)
        {
            return await _context.Albums
                .Where(s => s.GenreId == genreId)
                .Take(limit)
                .ToListAsync();
        }
    }

}
