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
            try
            {
                var genres = await _videoService.GetGenres();

                if (genres == null || !genres.Any())
                {
                    return NotFound("No genres found.");
                }

                return Ok(genres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

		// GET: api/Video/titles
		[HttpGet("titles")]
		public async Task<ActionResult<List<Video>>> GetTitles([FromQuery] string titleType, [FromQuery] string genre)
		{
            try
            {
                if (string.IsNullOrEmpty(titleType) || string.IsNullOrEmpty(genre))
                {
                    return BadRequest("Type or Genre parameter is missing.");
                }

                var titles = await _videoService.GetTitles(titleType, genre);

                if (titles == null || !titles.Any())
                {
                    return NotFound("No titles found.");
                }

                return Ok(titles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("titles/{titleIds}")]
        public async Task<ActionResult<List<Video>>> GetTitlesByIds(string titleIds)
        {
            try
            {
                var videos = await _videoService.GetTitlesByIds(titleIds);

                if (videos == null)
                {
                    return NotFound();
                }

                return Ok(videos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
		
}
