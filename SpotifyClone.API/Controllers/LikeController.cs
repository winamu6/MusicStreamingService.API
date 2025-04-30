using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Services.LikeServices.LikeInterfaces;
using Supabase.Gotrue;
using System.Security.Claims;

namespace SpotifyClone.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService; 
        private readonly UserManager<ApplicationUser> _userManager;

        public LikeController(ILikeService likeService, UserManager<ApplicationUser> userManager)
        {
            _likeService = likeService;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        }

        [HttpPost("{songId}")]
        public async Task<IActionResult> LikeSong(int songId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _likeService.LikeSongAsync(userId, songId);
            return success ? Ok() : BadRequest("Already liked or failed");
        }

        [HttpDelete("{songId}")]
        public async Task<IActionResult> UnlikeSong(int songId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _likeService.UnlikeSongAsync(userId, songId);
            return success ? Ok() : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetLikedSongs()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var songs = await _likeService.GetLikedSongsAsync(userId);
            return Ok(songs);
        }
    }
}
