using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.API.DTOs;
using SpotifyClone.API.Models;
using SpotifyClone.API.Utils;
using System.Security.Claims;
using System;
using SpotifyClone.API.Data;
using SpotifyClone.API.Services.SupabaseStorageServices;
using Microsoft.EntityFrameworkCore;

namespace SpotifyClone.API.Controllers
{
    [Authorize] [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ApplicationDbContext _context; private readonly ISupabaseStorageService _storage;

        public SongsController(ApplicationDbContext context, ISupabaseStorageService storage)
        {
            _context = context;
            _storage = storage;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadSong([FromForm] SongUploadDto dto)
        {
            if (!RoleChecker.IsMusicianOrAdmin(User))
                return Forbid();

            if (dto.AudioFile == null || dto.AudioFile.Length == 0)
                return BadRequest("Audio file is required.");

            if (!await _context.Albums.AnyAsync(a => a.Id == dto.AlbumId))
                return BadRequest("Album does not exist.");

            var audioFileName = $"audio_{Guid.NewGuid()}_{dto.AudioFile.FileName}";
            var audioPath = await _storage.UploadFileAsync("songs", audioFileName, dto.AudioFile.OpenReadStream());

            var song = new Song
            {
                Title = dto.Title,
                ArtistName = dto.ArtistName,
                Genre = dto.Genre,
                Duration = TimeSpan.FromSeconds(dto.Duration),
                AlbumId = dto.AlbumId,
                AudioFilePath = audioPath
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return Ok(song);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSong(int id, [FromForm] SongUploadDto dto)
        {
            if (!RoleChecker.IsMusicianOrAdmin(User))
                return Forbid();

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
                return NotFound();

            song.Title = dto.Title;
            song.ArtistName = dto.ArtistName;
            song.Genre = dto.Genre;
            song.Duration = TimeSpan.FromSeconds(dto.Duration);
            song.AlbumId = dto.AlbumId;

            if (dto.AudioFile != null)
            {
                await _storage.DeleteFileAsync("songs", song.AudioFilePath);
                var newAudioName = $"audio_{Guid.NewGuid()}_{dto.AudioFile.FileName}";
                song.AudioFilePath = await _storage.UploadFileAsync("songs", newAudioName, dto.AudioFile.OpenReadStream());
            }


            await _context.SaveChangesAsync();
            return Ok(song);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            if (!RoleChecker.IsMusicianOrAdmin(User))
                return Forbid();

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
                return NotFound();

            await _storage.DeleteFileAsync("songs", song.AudioFilePath);

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
