﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.API.Data;
using SpotifyClone.API.Models;
using SpotifyClone.API.Models.DTOs.AlbumDtos;
using SpotifyClone.API.Repositories.GenreRepositories.GenreRepositoriesInterfaces;
using SpotifyClone.API.Services.AlbumServices.AlbumInterfaces;
using SpotifyClone.API.Services.SupabaseStorageServices;
using SpotifyClone.API.Utils;
using Supabase.Gotrue;
using System;

namespace SpotifyClone.API.Controllers.AlbumController
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumService _albumService;
        private readonly IGenreRepository _genreRepository;

        public AlbumsController(
            IAlbumService albumService,
            IGenreRepository genreRepository)
        {
            _albumService = albumService;
            _genreRepository = genreRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAlbum([FromForm] AlbumUploadDto dto)
        {
            var album = await _albumService.CreateAlbumAsync(dto, User);
            return Ok(album);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(int id, [FromForm] AlbumUploadDto dto)
        {
            var album = await _albumService.UpdateAlbumAsync(id, dto, User);
            return Ok(album);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            await _albumService.DeleteAlbumAsync(id, User);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbum(int id)
        {
            var album = await _albumService.GetAlbumAsync(id);
            if (album == null)
                return NotFound();

            return Ok(album);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAlbums()
        {
            var albums = await _albumService.GetAllAlbumsAsync();
            return Ok(albums);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAlbums(
            [FromQuery] string? query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] bool descending = false)
        {
            var result = await _albumService.SearchAlbumsAsync(query, page, pageSize, sortBy, descending);
            return Ok(result);
        }
    }
}
