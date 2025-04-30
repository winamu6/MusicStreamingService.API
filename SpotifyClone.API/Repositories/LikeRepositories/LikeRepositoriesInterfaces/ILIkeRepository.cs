using SpotifyClone.API.Models.Entities;

namespace SpotifyClone.API.Repositories.LikeRepositories.LikeRepositoriesInterfaces
{
    public interface ILikeRepository { 
        Task<bool> AddLikeAsync(string userId, int songId); 
        Task<bool> RemoveLikeAsync(string userId, int songId); 
        Task<IEnumerable<Song>> GetLikedSongsAsync(string userId); 
        Task<bool> LikeExistsAsync(string userId, int songId); 
    }
}
