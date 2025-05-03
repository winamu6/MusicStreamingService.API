using SpotifyClone.API.Models.Common;
using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using System.Security.Claims;

namespace SpotifyClone.API.Services.SongServices.SongInterfaces
{
    public interface ISongService
    {
        Task<Song> UploadSongAsync(SongUploadDto dto, ClaimsPrincipal user);
        Task<Song> EditSongAsync(int id, SongUploadDto dto, ClaimsPrincipal user);
        Task DeleteSongAsync(int id, ClaimsPrincipal user);
        Task<PagedResult<Song>> SearchSongsAsync(string? query, int page, int pageSize, string? sortBy, bool descending);
        Task<string> ListenToSongAsync(int id, ClaimsPrincipal user);
        Task<List<ListeningHistoryDto>> GetListeningHistoryAsync(ClaimsPrincipal user);
        Task<List<Song>> GetRecommendationsAsync(ClaimsPrincipal user, int limit = 100);

    }

}
