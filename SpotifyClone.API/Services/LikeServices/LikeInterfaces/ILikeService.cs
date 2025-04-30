using SpotifyClone.API.Models.Entities;

namespace SpotifyClone.API.Services.LikeServices.LikeInterfaces
{
    public interface ILikeService { 
        Task<bool> LikeSongAsync(string userId, int songId); 
        Task<bool> UnlikeSongAsync(string userId, int songId); 
        Task<IEnumerable<Song>> GetLikedSongsAsync(string userId); 
    }
}
