using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.DTOs;
using SpotifyClone.API.Models;
using SpotifyClone.API.Services.SupabaseStorageServices;
using Supabase.Gotrue;

namespace SpotifyClone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISupabaseStorageService _storage;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlaylistsController(ApplicationDbContext context, ISupabaseStorageService storage, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _storage = storage;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromForm] PlaylistCreateDto dto)
        {
            var user = await _userManager.GetUserAsync(User);

            string? coverPath = null;
            if (dto.CoverImage != null)
            {
                var fileName = $"playlist_{Guid.NewGuid()}_{dto.CoverImage.FileName}";
                coverPath = await _storage.UploadFileAsync("playlists", fileName, dto.CoverImage.OpenReadStream());
            }

            var playlist = new Playlist
            {
                Name = dto.Name,
                UserId = user.Id,
                CoverImagePath = coverPath
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            var playlistDto = new PlaylistDto
            {
                Id = playlist.Id,
                Name = playlist.Name,
                CoverImagePath = playlist.CoverImagePath
            };

            return Ok(playlistDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var playlist = await _context.Playlists.Include(p => p.PlaylistSongs).FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
                return NotFound();
            if (playlist.UserId != user.Id)
                return Forbid();

            if (!string.IsNullOrEmpty(playlist.CoverImagePath))
                await _storage.DeleteFileAsync("playlists", playlist.CoverImagePath);

            _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, int songId)
        {
            var user = await _userManager.GetUserAsync(User);
            var playlist = await _context.Playlists.FindAsync(playlistId);

            if (playlist == null)
                return NotFound();
            if (playlist.UserId != user.Id)
                return Forbid();

            if (await _context.PlaylistSongs.AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId))
                return BadRequest("Song already in playlist");

            _context.PlaylistSongs.Add(new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var user = await _userManager.GetUserAsync(User);
            var playlist = await _context.Playlists.FindAsync(playlistId);

            if (playlist == null)
                return NotFound();
            if (playlist.UserId != user.Id)
                return Forbid();

            var link = await _context.PlaylistSongs.FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (link == null)
                return NotFound();

            _context.PlaylistSongs.Remove(link);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPlaylists([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var playlists = await _context.Playlists
                .Where(a => a.Name.Contains(query))
                .ToListAsync();

            return Ok(playlists);
        }

    }
}
