using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using System.Security.Claims;

namespace SpotifyClone.API.Services.PlaylistServices.PlaylistInterfaces
{
    public interface IPlaylistService
    {
        Task<PlaylistDto> CreatePlaylistAsync(PlaylistCreateDto dto, ClaimsPrincipal user);
        Task DeletePlaylistAsync(int id, ClaimsPrincipal user);
        Task AddSongToPlaylistAsync(int playlistId, int songId, ClaimsPrincipal user);
        Task RemoveSongFromPlaylistAsync(int playlistId, int songId, ClaimsPrincipal user);
        Task<List<Playlist>> SearchPlaylistsAsync(string query);
    }

}
