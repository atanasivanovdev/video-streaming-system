using WebApp.Models;

namespace WebApp.Services
{
    public interface IWatchlistService
    {
        public Task<WatchlistResult> AddTitleToWatchlist(WatchlistModel watchlistModel);
        public Task<WatchlistResult> GetTitleFromWatchlist(string userId);
        public Task<WatchlistResult> RemoveTitleFromWatchlist(WatchlistModel watchlistModel);
    }
}
