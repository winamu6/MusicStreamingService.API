using SpotifyClone.API.Models.Entities;

namespace SpotifyClone.API.Repositories.GenreRepositories.GenreRepositoriesInterfaces
{
    public interface IGenreRepository
    {
        Task<Genre> GetOrCreateGenreAsync(string genreName);
        Task<List<Genre>> GetAllGenresAsync();
        Task<List<Song>> GetTopSongsByGenreAsync(int genreId, int limit);
        Task<List<Album>> GetAlbumsByGenreAsync(int genreId, int limit);
    }

}
