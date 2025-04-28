using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.Models;
using SpotifyClone.API.Models.DTOs;
using SpotifyClone.API.Services.PlaylistServices.PlaylistInterfaces;
using SpotifyClone.API.Services.SupabaseStorageServices;
using Supabase.Gotrue;

namespace SpotifyClone.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromForm] PlaylistCreateDto dto)
        {
            var playlist = await _playlistService.CreatePlaylistAsync(dto, User);
            return Ok(playlist);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            await _playlistService.DeletePlaylistAsync(id, User);
            return NoContent();
        }

        [HttpPost("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, int songId)
        {
            await _playlistService.AddSongToPlaylistAsync(playlistId, songId, User);
            return Ok();
        }

        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            await _playlistService.RemoveSongFromPlaylistAsync(playlistId, songId, User);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPlaylists([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var playlists = await _playlistService.SearchPlaylistsAsync(query);
            return Ok(playlists);
        }
    }
}
