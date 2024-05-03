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
            await _watchlistService.AddTitleToWatchlist(watchlist.UserId, watchlist.TitleId);
            return Ok(watchlist.TitleId);
        }

        // GET api/watchlist/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<string>>> GetWatchlist(string userId)
        {
            var titles = await _watchlistService.GetTitlesFromWatchlist(userId);
            if (titles == null || titles.Count == 0)
                return NotFound("Watchlist is empty.");

            return Ok(titles);
        }

        // DELETE api/watchlist/remove
        [HttpDelete("remove")]
        [HttpDelete("remove/{userId}/{titleId}")]
        public async Task<IActionResult> RemoveFromWatchlist(string userId, string titleId)
        {
            await _watchlistService.RemoveTitleFromWatchlist(userId, titleId);
            return Ok(titleId);
        }
    }
}
