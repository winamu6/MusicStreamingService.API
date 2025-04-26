using SpotifyClone.API.Models.Common;
using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.SongRepositories.SongRepositoriesInterfaces;
using SpotifyClone.API.Services.SongServices.SongInterfaces;
using SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces;
using SpotifyClone.API.Utils;
using System.Security.Claims;

namespace SpotifyClone.API.Services.SongServices
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly ISupabaseStorageService _storage;

        public SongService(ISongRepository songRepository, ISupabaseStorageService storage)
        {
            _songRepository = songRepository;
            _storage = storage;
        }

        public async Task<Song> UploadSongAsync(SongUploadDto dto, ClaimsPrincipal user)
        {
            if (!RoleChecker.IsMusicianOrAdmin(user))
                throw new UnauthorizedAccessException();

            if (dto.AudioFile == null || dto.AudioFile.Length == 0)
                throw new ArgumentException("Audio file is required.");

            if (!await _songRepository.AlbumExistsAsync(dto.AlbumId))
                throw new ArgumentException("Album does not exist.");

            var fileName = $"audio_{Guid.NewGuid()}_{dto.AudioFile.FileName}";
            var audioPath = await _storage.UploadFileAsync("songs", fileName, dto.AudioFile.OpenReadStream());

            var song = new Song
            {
                Title = dto.Title,
                ArtistName = dto.ArtistName,
                Genre = dto.Genre,
                Duration = TimeSpan.FromSeconds(dto.Duration),
                AlbumId = dto.AlbumId,
                AudioFilePath = audioPath
            };

            await _songRepository.AddSongAsync(song);
            return song;
        }

        public async Task<Song> EditSongAsync(int id, SongUploadDto dto, ClaimsPrincipal user)
        {
            if (!RoleChecker.IsMusicianOrAdmin(user))
                throw new UnauthorizedAccessException();

            var song = await _songRepository.GetSongByIdAsync(id);
            if (song == null)
                throw new KeyNotFoundException("Song not found.");

            song.Title = dto.Title;
            song.ArtistName = dto.ArtistName;
            song.Genre = dto.Genre;
            song.Duration = TimeSpan.FromSeconds(dto.Duration);
            song.AlbumId = dto.AlbumId;

            if (dto.AudioFile != null)
            {
                await _storage.DeleteFileAsync("songs", song.AudioFilePath);
                var newFileName = $"audio_{Guid.NewGuid()}_{dto.AudioFile.FileName}";
                song.AudioFilePath = await _storage.UploadFileAsync("songs", newFileName, dto.AudioFile.OpenReadStream());
            }

            await _songRepository.UpdateSongAsync(song);
            return song;
        }

        public async Task DeleteSongAsync(int id, ClaimsPrincipal user)
        {
            if (!RoleChecker.IsMusicianOrAdmin(user))
                throw new UnauthorizedAccessException();

            var song = await _songRepository.GetSongByIdAsync(id);
            if (song == null)
                throw new KeyNotFoundException("Song not found.");

            await _storage.DeleteFileAsync("songs", song.AudioFilePath);
            await _songRepository.DeleteSongAsync(song);
        }

        public async Task<PagedResult<Song>> SearchSongsAsync(string? query, int page, int pageSize, string? sortBy, bool descending)
        {
            return await _songRepository.SearchSongsAsync(query, page, pageSize, sortBy, descending);
        }

        public async Task<string> ListenToSongAsync(int id, ClaimsPrincipal user)
        {
            var song = await _songRepository.GetSongByIdAsync(id);
            if (song == null)
                throw new KeyNotFoundException("Song not found.");

            song.ListenCount++;
            await _songRepository.UpdateSongAsync(song);

            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var history = new ListeningHistory
                    {
                        UserId = userId,
                        SongId = song.Id,
                        ListenedAt = DateTime.UtcNow
                    };
                    await _songRepository.AddListeningHistoryAsync(history);
                }
            }

            return await _storage.GetPublicUrlAsync("songs", song.AudioFilePath);
        }

        public async Task<List<ListeningHistoryDto>> GetListeningHistoryAsync(ClaimsPrincipal user)
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

            return await _songRepository.GetListeningHistoryAsync(userId);
        }
    }

}
