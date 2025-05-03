using SpotifyClone.API.Models.Common;
using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;

namespace SpotifyClone.API.Repositories.SongRepositories.SongRepositoriesInterfaces
{
    public interface ISongRepository
    {
        Task<bool> AlbumExistsAsync(int albumId);
        Task AddSongAsync(Song song);
        Task<Song?> GetSongByIdAsync(int id);
        Task UpdateSongAsync(Song song);
        Task DeleteSongAsync(Song song);
        Task<PagedResult<Song>> SearchSongsAsync(string? query, int page, int pageSize, string? sortBy, bool descending);
        Task AddListeningHistoryAsync(ListeningHistory history);
        Task<List<ListeningHistoryDto>> GetListeningHistoryAsync(string userId);
        Task<List<Song>> GetContentBasedRecommendationsAsync(Song referenceSong, int maxResults = 100);
        Task<List<int>> GetUserListenedSongIdsAsync(string userId);
        Task<List<SongFeatureDto>> GetRecentSongFeaturesAsync(string userId, int count);
        Task<List<Song>> GetSimilarSongsAsync(float tempo, float energy, float danceability, List<int> excludeSongIds, int limit);

    }

}
