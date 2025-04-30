using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.LikeRepositories.LikeRepositoriesInterfaces;

namespace SpotifyClone.API.Repositories.LikeRepositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _context;

        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddLikeAsync(string userId, int songId)
        {
            if (await LikeExistsAsync(userId, songId))
                return false;

            var like = new Like { UserId = userId, SongId = songId };
            _context.Likes.Add(like);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveLikeAsync(string userId, int songId)
        {
            var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.SongId == songId);
            if (like == null)
                return false;

            _context.Likes.Remove(like);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Song>> GetLikedSongsAsync(string userId)
        {
            return await _context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.Song)
                .ToListAsync();
        }

        public async Task<bool> LikeExistsAsync(string userId, int songId)
        {
            return await _context.Likes.AnyAsync(l => l.UserId == userId && l.SongId == songId);
        }

    }
}
