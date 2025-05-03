using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.API.Models;
using SpotifyClone.API.Utils;
using System.Security.Claims;
using System;
using SpotifyClone.API.Data;
using SpotifyClone.API.Services.SupabaseStorageServices;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Services.SongServices.SongInterfaces;
using SpotifyClone.API.Models.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SpotifyClone.API.Repositories.GenreRepositories.GenreRepositoriesInterfaces;

namespace SpotifyClone.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly IGenreRepository _genreRepository;

        public SongsController(
            ISongService songService,
            IGenreRepository genreRepository)
        {
            _songService = songService;
            _genreRepository = genreRepository;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadSong([FromForm] SongUploadDto dto)
        {
            var song = await _songService.UploadSongAsync(dto, User);
            return Ok(song);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSong(int id, [FromForm] SongUploadDto dto)
        {
            var song = await _songService.EditSongAsync(id, dto, User);
            return Ok(song);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            await _songService.DeleteSongAsync(id, User);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchSongs(
            [FromQuery] string? query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] bool descending = false)
        {
            var result = await _songService.SearchSongsAsync(query, page, pageSize, sortBy, descending);
            return Ok(result);
        }

        [HttpGet("{id}/listen")]
        public async Task<IActionResult> ListenToSong(int id)
        {
            var url = await _songService.ListenToSongAsync(id, User);
            return Ok(new { url });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetListeningHistory()
        {
            var history = await _songService.GetListeningHistoryAsync(User);
            return Ok(history);
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _genreRepository.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet("genres/{genreId}/top")]
        public async Task<IActionResult> GetTopSongsByGenre(int genreId)
        {
            var songs = await _genreRepository.GetTopSongsByGenreAsync(genreId, 100);
            return Ok(songs);
        }

        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations()
        {
            var songs = await _songService.GetRecommendationsAsync(User);
            return Ok(songs);
        }

    }
}
