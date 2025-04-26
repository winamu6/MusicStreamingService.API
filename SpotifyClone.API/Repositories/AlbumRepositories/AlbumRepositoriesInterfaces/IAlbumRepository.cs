using SpotifyClone.API.Models.Entities;

namespace SpotifyClone.API.Repositories.AlbumRepositories.AlbumRepositoriesInterfaces
{
    public interface IAlbumRepository
    {
        Task<Album?> GetAlbumByIdAsync(int id);
        Task<Album?> GetAlbumWithSongsAsync(int id);
        Task<List<Album>> GetAllAlbumsAsync();
        Task AddAlbumAsync(Album album);
        Task UpdateAlbumAsync(Album album);
        Task DeleteAlbumAsync(Album album);
        Task<object> SearchAlbumsAsync(string? query, int page, int pageSize, string? sortBy, bool descending);
    }

}
