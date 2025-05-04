using SpotifyClone.API.Models.Common;
using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.GenreRepositories.GenreRepositoriesInterfaces;
using SpotifyClone.API.Repositories.SongRepositories.SongRepositoriesInterfaces;
using SpotifyClone.API.Services.SongServices.SongInterfaces;
using SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces;
using SpotifyClone.API.Utils;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace SpotifyClone.API.Services.SongServices
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly ISupabaseStorageService _storage;
        private readonly IGenreRepository _genreRepository;
        private readonly HttpClient _httpClient;

        public SongService(
            ISongRepository songRepository, 
            ISupabaseStorageService storage,
            IGenreRepository genreRepository,
            HttpClient httpClient)
        {
            _songRepository = songRepository;
            _storage = storage;
            _genreRepository = genreRepository;
            _httpClient = httpClient;
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

            var pythonResult = await AnalyzeAudioFile(dto.AudioFile);

            var genre = await _genreRepository.GetOrCreateGenreAsync(dto.GenreName);

            var song = new Song
            {
                Title = dto.Title,
                ArtistName = dto.ArtistName,
                Duration = TimeSpan.FromSeconds(pythonResult.Duration),
                AlbumId = dto.AlbumId,
                GenreId = genre.Id,
                AudioFilePath = audioPath,
                Tempo = pythonResult.Tempo,
                Energy = pythonResult.Energy,
                Danceability = pythonResult.Danceability
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

            var genre = await _genreRepository.GetOrCreateGenreAsync(dto.GenreName);

            song.Title = dto.Title;
            song.ArtistName = dto.ArtistName;
            song.GenreId = genre.Id;
            song.Duration = TimeSpan.FromSeconds(dto.Duration);
            song.AlbumId = dto.AlbumId;
            song.Tempo = dto.Tempo;
            song.Energy = dto.Energy;
            song.Danceability = dto.Danceability;


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

        public async Task<List<Song>> GetRecommendationsAsync(ClaimsPrincipal user, int limit = 100)
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

            var listenedSongIds = await _songRepository
                .GetUserListenedSongIdsAsync(userId);

            var recentFeatures = await _songRepository
                .GetRecentSongFeaturesAsync(userId, count: 10);

            if (!recentFeatures.Any())
                return new List<Song>();

            var avgTempo = recentFeatures.Average(f => f.Tempo);
            var avgEnergy = recentFeatures.Average(f => f.Energy);
            var avgDance = recentFeatures.Average(f => f.Danceability);

            var recommendations = await _songRepository
                .GetSimilarSongsAsync(avgTempo, avgEnergy, avgDance, listenedSongIds, limit);

            return recommendations;
        }

        private async Task<(float Tempo, float Energy, float Danceability, double Duration)> AnalyzeAudioFile(IFormFile audioFile)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(audioFile.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/mpeg");
            content.Add(fileContent, "file", audioFile.FileName);

            var response = await _httpClient.PostAsync("http://localhost:5000/analyze", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to analyze audio file.");
            }

            var result = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(result);

            float tempo = json.GetProperty("tempo").GetSingle();
            float energy = json.GetProperty("energy").GetSingle();
            float danceability = json.GetProperty("danceability").GetSingle();
            double duration = json.GetProperty("duration_seconds").GetDouble();

            return (tempo, energy, danceability, duration);
        }
    }

}
