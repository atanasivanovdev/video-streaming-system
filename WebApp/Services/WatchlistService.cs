using System.Net.Http.Json;
using WebApp.Models;

namespace WebApp.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly HttpClient _httpClient;

        public WatchlistService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WatchlistResult> AddTitleToWatchlist(WatchlistModel watchlistModel)
        {
            var result = await _httpClient.PostAsJsonAsync("gateway/watchlist/add", watchlistModel);

            WatchlistResult watchlistResult = new WatchlistResult();

            watchlistResult.Successful = result.IsSuccessStatusCode;

            if (!watchlistResult.Successful)
            {
                var error = await result.Content.ReadAsStringAsync();
                watchlistResult.Error = error;
                return watchlistResult;
            }

            watchlistResult.Watchlist = null!;

            return watchlistResult;
        }

        public async Task<WatchlistResult> GetTitleFromWatchlist(string userId)
        {
            var result = await _httpClient.GetAsync($"gateway/watchlist/{userId}");

            WatchlistResult watchlistResult = new WatchlistResult();

            watchlistResult.Successful = result.IsSuccessStatusCode;

            if (!watchlistResult.Successful)
            {
                var error = await result.Content.ReadAsStringAsync();
                watchlistResult.Watchlist = new List<Video>();
                watchlistResult.Error = error;
                return watchlistResult;
            }

            watchlistResult.Watchlist = await result.Content.ReadFromJsonAsync<List<Video>>();

            return watchlistResult;
        }

        public async Task<WatchlistResult> RemoveTitleFromWatchlist(WatchlistModel watchlistModel)
        {
            HttpResponseMessage result = await _httpClient.DeleteAsync($"gateway/watchlist/remove/{watchlistModel.UserId}/{watchlistModel.TitleId}");

            WatchlistResult watchlistResult = new WatchlistResult();

            watchlistResult.Successful = result.IsSuccessStatusCode;

            // If the request was not successful, read the error message from the response.
            if (!watchlistResult.Successful)
            {
                var error = await result.Content.ReadAsStringAsync();
                watchlistResult.Error = error;
                return watchlistResult;
            }

            return watchlistResult;
        }
    }
}
