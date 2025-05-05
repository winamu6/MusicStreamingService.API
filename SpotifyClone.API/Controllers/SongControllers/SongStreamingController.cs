using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.API.Services.SongServices.SongInterfaces;
using Supabase.Gotrue;

namespace SpotifyClone.API.Controllers.SongControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/songs")]
    public class SongStreamingController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongStreamingController(ISongService songService)
        {
            _songService = songService;
        }

        [AllowAnonymous]
        [HttpGet("{id}/stream")]
        public async Task<IActionResult> StreamSong(int id)
        {
            try
            {
                var stream = await _songService.GetSongStreamAsync(id, HttpContext.User);

                return File(stream, "audio/mpeg", enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

}
