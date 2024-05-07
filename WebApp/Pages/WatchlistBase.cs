using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class WatchlistBase: ComponentBase
    {
        [Inject]
        public IWatchlistService WatchlistService { get; set; }

        [Inject]
        public IAuthService AuthService { get; set; }

        [Inject]
        public IWebAssemblyHostEnvironment Environment { get; set; }

        public WatchlistResult Watchlist { get; set; }
        private string userId = "";
		public string selectedTitleId;
		public double selectedPrice;

		protected override async Task OnInitializedAsync()
        {
            userId = await AuthService.GetUserId();
            if (userId == null) return;

            Watchlist = await WatchlistService.GetTitleFromWatchlist(userId);
        }

        public string GetImagePath()
        {
            string webRootPath = Environment.BaseAddress;
            return Path.Combine(webRootPath, "no-image.jpg");
        }

        public async void OnRemoveVideo(string titleId)
        {
            WatchlistModel watchlistModel = new WatchlistModel();
            watchlistModel.TitleId = titleId;
            watchlistModel.UserId = userId;
            await WatchlistService.RemoveTitleFromWatchlist(watchlistModel);
            Watchlist = await WatchlistService.GetTitleFromWatchlist(userId);
            StateHasChanged();
        }

		public void SelectVideoForPayment(string titleId, double price)
		{
			selectedTitleId = titleId;
			selectedPrice = price;
		}
	}
}
