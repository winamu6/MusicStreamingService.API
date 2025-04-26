using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using System.Security.Claims;

namespace SpotifyClone.API.Services.AlbumServices.AlbumInterfaces
{
    public interface IAlbumService
    {
        Task<Album> CreateAlbumAsync(AlbumUploadDto dto, ClaimsPrincipal user);
        Task<Album> UpdateAlbumAsync(int id, AlbumUploadDto dto, ClaimsPrincipal user);
        Task DeleteAlbumAsync(int id, ClaimsPrincipal user);
        Task<Album?> GetAlbumAsync(int id);
        Task<List<Album>> GetAllAlbumsAsync();
        Task<object> SearchAlbumsAsync(string? query, int page, int pageSize, string? sortBy, bool descending);
    }

}
