using Microsoft.AspNetCore.Identity;
using SpotifyClone.API.Models.DTOs.PlaylistDtos;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.PlaylistRepositories.PlaylistRepositoriesInterfaces;
using SpotifyClone.API.Services.PlaylistServices.PlaylistInterfaces;
using SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces;
using System.Security.Claims;

namespace SpotifyClone.API.Services.PlaylistServices
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ISupabaseStorageService _storage;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlaylistService(IPlaylistRepository playlistRepository, ISupabaseStorageService storage, UserManager<ApplicationUser> userManager)
        {
            _playlistRepository = playlistRepository;
            _storage = storage;
            _userManager = userManager;
        }

        public async Task<PlaylistDto> CreatePlaylistAsync(PlaylistCreateDto dto, ClaimsPrincipal user)
        {
            var appUser = await _userManager.GetUserAsync(user);

            string? coverPath = null;
            if (dto.CoverImage != null)
            {
                var fileName = $"playlist_{Guid.NewGuid()}_{dto.CoverImage.FileName}";
                coverPath = await _storage.UploadFileAsync("playlists", fileName, dto.CoverImage.OpenReadStream());
            }

            var playlist = new Playlist
            {
                Name = dto.Name,
                UserId = appUser.Id,
                CoverImagePath = coverPath
            };

            await _playlistRepository.AddPlaylistAsync(playlist);

            return new PlaylistDto
            {
                Id = playlist.Id,
                Name = playlist.Name,
                CoverImagePath = playlist.CoverImagePath
            };
        }

        public async Task DeletePlaylistAsync(int id, ClaimsPrincipal user)
        {
            var appUser = await _userManager.GetUserAsync(user);
            var playlist = await _playlistRepository.GetPlaylistWithSongsAsync(id);

            if (playlist == null)
                throw new KeyNotFoundException("Playlist not found");

            if (playlist.UserId != appUser.Id)
                throw new UnauthorizedAccessException();

            if (!string.IsNullOrEmpty(playlist.CoverImagePath))
                await _storage.DeleteFileAsync("playlists", playlist.CoverImagePath);

            await _playlistRepository.DeletePlaylistAsync(playlist);
        }

        public async Task AddSongToPlaylistAsync(int playlistId, int songId, ClaimsPrincipal user)
        {
            var appUser = await _userManager.GetUserAsync(user);
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Playlist not found");

            if (playlist.UserId != appUser.Id)
                throw new UnauthorizedAccessException();

            if (await _playlistRepository.IsSongInPlaylistAsync(playlistId, songId))
                throw new InvalidOperationException("Song already in playlist");

            await _playlistRepository.AddSongToPlaylistAsync(playlistId, songId);
        }

        public async Task RemoveSongFromPlaylistAsync(int playlistId, int songId, ClaimsPrincipal user)
        {
            var appUser = await _userManager.GetUserAsync(user);
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Playlist not found");

            if (playlist.UserId != appUser.Id)
                throw new UnauthorizedAccessException();

            var link = await _playlistRepository.GetPlaylistSongLinkAsync(playlistId, songId);
            if (link == null)
                throw new KeyNotFoundException("Song not found in playlist");

            await _playlistRepository.RemoveSongFromPlaylistAsync(link);
        }

        public async Task<List<Playlist>> SearchPlaylistsAsync(string query)
        {
            return await _playlistRepository.SearchPlaylistsAsync(query);
        }
    }

}
