using Microsoft.AspNetCore.Mvc;
using RestSharp;
using WatchlistService.Models;
using WatchlistService.Services;

namespace WatchlistService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchlistController : ControllerBase
    {
        private readonly UserWatchlistService _watchlistService;

        public WatchlistController(UserWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }

        // POST api/watchlist/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToWatchlist([FromBody] WatchlistDTO watchlist)
        {
            try
            {
                await _watchlistService.AddTitleToWatchlist(watchlist.UserId, watchlist.TitleId);
                return Ok(watchlist.TitleId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/watchlist/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWatchlist(string userId)
        {
            try
            {
                var titles = await _watchlistService.GetTitlesFromWatchlist(userId);
                if (titles == null || titles.Count == 0)
                {
                    return NotFound("Watchlist is empty.");
                }

                return Ok(titles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/watchlist/remove/{userId}/{titleId}
        [HttpDelete("remove/{userId}/{titleId}")]
        public async Task<IActionResult> RemoveFromWatchlist(string userId, string titleId)
        {
            try
            {
                await _watchlistService.RemoveTitleFromWatchlist(userId, titleId);
                return Ok(titleId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
