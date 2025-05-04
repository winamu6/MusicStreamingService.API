using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Repositories.AlbumRepositories.AlbumRepositoriesInterfaces;
using SpotifyClone.API.Repositories.GenreRepositories.GenreRepositoriesInterfaces;
using SpotifyClone.API.Services.AlbumServices.AlbumInterfaces;
using SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces;
using SpotifyClone.API.Utils;
using System.Security.Claims;

namespace SpotifyClone.API.Services.AlbumServices
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly ISupabaseStorageService _storage;
        private readonly IGenreRepository _genreRepository;

        public AlbumService(
            IAlbumRepository albumRepository, 
            ISupabaseStorageService storage,
            IGenreRepository genreRepository)
        {
            _albumRepository = albumRepository;
            _storage = storage;
            _genreRepository = genreRepository;
        }

        public async Task<Album> CreateAlbumAsync(AlbumUploadDto dto, ClaimsPrincipal user)
        {
            if (!RoleChecker.IsMusicianOrAdmin(user))
                throw new UnauthorizedAccessException();

            string? coverPath = null;

            if (dto.CoverImage != null)
            {
                var fileName = $"album_cover_{Guid.NewGuid()}_{dto.CoverImage.FileName}";
                coverPath = await _storage.UploadFileAsync("albums", fileName, dto.CoverImage.OpenReadStream());
            }

            var genre = await _genreRepository.GetOrCreateGenreAsync(dto.GenreName);

            var album = new Album
            {
                Title = dto.Title,
                ArtistName = dto.ArtistName,
                ReleaseDate = dto.ReleaseDate,
                CoverImagePath = coverPath,
                Genre = genre
            };

            await _albumRepository.AddAlbumAsync(album);
            return album;
        }

        public async Task<Album> UpdateAlbumAsync(int id, AlbumUploadDto dto, ClaimsPrincipal user)
        {
            if (!RoleChecker.IsMusicianOrAdmin(user))
                throw new UnauthorizedAccessException();

            var album = await _albumRepository.GetAlbumByIdAsync(id);
            if (album == null)
                throw new KeyNotFoundException("Album not found");

            var genre = await _genreRepository.GetOrCreateGenreAsync(dto.GenreName);

            album.Title = dto.Title;
            album.ArtistName = dto.ArtistName;
            album.ReleaseDate = dto.ReleaseDate;
            album.GenreId = genre.Id;

            if (dto.CoverImage != null)
            {
                if (!string.IsNullOrEmpty(album.CoverImagePath))
                    await _storage.DeleteFileAsync("albums", album.CoverImagePath);

                var fileName = $"album_cover_{Guid.NewGuid()}_{dto.CoverImage.FileName}";
                album.CoverImagePath = await _storage.UploadFileAsync("albums", fileName, dto.CoverImage.OpenReadStream());
            }

            await _albumRepository.UpdateAlbumAsync(album);
            return album;
        }

        public async Task DeleteAlbumAsync(int id, ClaimsPrincipal user)
        {
            if (!RoleChecker.IsMusicianOrAdmin(user))
                throw new UnauthorizedAccessException();

            var album = await _albumRepository.GetAlbumWithSongsAsync(id);
            if (album == null)
                throw new KeyNotFoundException("Album not found");

            if (album.Songs.Any())
                throw new InvalidOperationException("Album contains songs. Delete them first.");

            if (!string.IsNullOrEmpty(album.CoverImagePath))
                await _storage.DeleteFileAsync("albums", album.CoverImagePath);

            await _albumRepository.DeleteAlbumAsync(album);
        }

        public async Task<Album?> GetAlbumAsync(int id)
        {
            return await _albumRepository.GetAlbumWithSongsAsync(id);
        }

        public async Task<List<Album>> GetAllAlbumsAsync()
        {
            return await _albumRepository.GetAllAlbumsAsync();
        }

        public async Task<object> SearchAlbumsAsync(string? query, int page, int pageSize, string? sortBy, bool descending)
        {
            return await _albumRepository.SearchAlbumsAsync(query, page, pageSize, sortBy, descending);
        }
    }

}
