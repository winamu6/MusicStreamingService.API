using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.DTOs;
using SpotifyClone.API.Models;
using SpotifyClone.API.Services.SupabaseStorageServices;
using SpotifyClone.API.Utils;
using Supabase.Gotrue;
using System;

namespace SpotifyClone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AlbumsController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 
        private readonly ISupabaseStorageService _storage;

        public AlbumsController(ApplicationDbContext context, ISupabaseStorageService storage)
        {
            _context = context;
            _storage = storage;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAlbum([FromForm] AlbumUploadDto dto)
        {
            if (!RoleChecker.IsMusicianOrAdmin(User))
                return Forbid();

            string? coverPath = null;

            if (dto.CoverImage != null)
            {
                var fileName = $"album_cover_{Guid.NewGuid()}_{dto.CoverImage.FileName}";
                coverPath = await _storage.UploadFileAsync("albums", fileName, dto.CoverImage.OpenReadStream());
            }

            var album = new Album
            {
                Title = dto.Title,
                ArtistName = dto.ArtistName,
                ReleaseDate = dto.ReleaseDate,
                CoverImagePath = coverPath
            };

            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            return Ok(album);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(int id, [FromForm] AlbumUploadDto dto)
        {
            if (!RoleChecker.IsMusicianOrAdmin(User))
                return Forbid();

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
                return NotFound();

            album.Title = dto.Title;
            album.ArtistName = dto.ArtistName;
            album.ReleaseDate = dto.ReleaseDate;

            if (dto.CoverImage != null)
            {
                if (!string.IsNullOrEmpty(album.CoverImagePath))
                    await _storage.DeleteFileAsync("albums", album.CoverImagePath);

                var fileName = $"album_cover_{Guid.NewGuid()}_{dto.CoverImage.FileName}";
                album.CoverImagePath = await _storage.UploadFileAsync("albums", fileName, dto.CoverImage.OpenReadStream());
            }

            await _context.SaveChangesAsync();
            return Ok(album);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            if (!RoleChecker.IsMusicianOrAdmin(User))
                return Forbid();

            var album = await _context.Albums.Include(a => a.Songs).FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
                return NotFound();

            if (!string.IsNullOrEmpty(album.CoverImagePath))
                await _storage.DeleteFileAsync("albums", album.CoverImagePath);

            if (album.Songs.Any())
                return BadRequest("Album contains songs. Delete them first.");

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbum(int id)
        {
            var album = await _context.Albums.Include(a => a.Songs).FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
                return NotFound();

            return Ok(album);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllAlbums()
        {
            var albums = await _context.Albums.Include(a => a.Songs).ToListAsync();
            return Ok(albums);
        }

    }
}
