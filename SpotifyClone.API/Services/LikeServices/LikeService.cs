using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.LikeRepositories.LikeRepositoriesInterfaces;
using SpotifyClone.API.Services.LikeServices.LikeInterfaces;

namespace SpotifyClone.API.Services.LikeServices
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;

        public LikeService(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public Task<bool> LikeSongAsync(string userId, int songId) => _likeRepository.AddLikeAsync(userId, songId);
        public Task<bool> UnlikeSongAsync(string userId, int songId) => _likeRepository.RemoveLikeAsync(userId, songId);
        public Task<IEnumerable<Song>> GetLikedSongsAsync(string userId) => _likeRepository.GetLikedSongsAsync(userId);

    }
}
