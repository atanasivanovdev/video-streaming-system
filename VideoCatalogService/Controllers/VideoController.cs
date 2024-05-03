using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using VideoCatalogService.Models;
using VideoCatalogService.Services;

namespace VideoCatalogService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VideoController : ControllerBase
	{
		private readonly VideoService _videoService;

		public VideoController(VideoService videoService)
		{
			_videoService = videoService;
		}


		// GET: api/Video/genres
		[HttpGet("genres")]
		public async Task<ActionResult<List<string>>> GetGenres()
		{
			var genres = await _videoService.GetGenres();

			if (genres == null || !genres.Any())
			{
				return NotFound("No genres found.");
			}

			return Ok(genres);
		}

		// GET: api/Video/titles
		[HttpGet("titles")]
		public async Task<ActionResult<List<Video>>> GetTitles([FromQuery] string titleType, [FromQuery] string genre)
		{
			if (string.IsNullOrEmpty(titleType) || string.IsNullOrEmpty(genre))
			{
				return BadRequest("Type or Genre parameter is missing.");
			}

			var titles = await _videoService.GetTitles(titleType, genre);

			if (titles == null || !titles.Any())
			{
				return NotFound("No genres found.");
			}

			return Ok(titles);
		}

        [HttpGet("titles/{titleId}")]
        public async Task<ActionResult<Video>> GetTitle(string titleId)
        {
            var video = await _videoService.GetTitle(titleId);

            if (video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }
    }
		
}
