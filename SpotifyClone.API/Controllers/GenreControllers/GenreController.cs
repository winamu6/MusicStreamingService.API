using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.API.Repositories.AlbumRepositories.AlbumRepositoriesInterfaces;
using SpotifyClone.API.Repositories.GenreRepositories.GenreRepositoriesInterfaces;
using SpotifyClone.API.Repositories.SongRepositories.SongRepositoriesInterfaces;

namespace SpotifyClone.API.Controllers.GenreControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _genreRepository;

        public GenreController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _genreRepository.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet("Song/{genreId}/top")]
        public async Task<IActionResult> GetTopSongsByGenre(int genreId)
        {
            var songs = await _genreRepository.GetTopSongsByGenreAsync(genreId, 100);
            return Ok(songs);
        }

        [HttpGet("Album/{genreId}/top")]
        public async Task<IActionResult> GetAlbumByGenre(int genreId)
        {
            var album = await _genreRepository.GetAlbumsByGenreAsync(genreId, 100);
            if (album == null)
                return NotFound();

            return Ok(album);
        }
    }
}
