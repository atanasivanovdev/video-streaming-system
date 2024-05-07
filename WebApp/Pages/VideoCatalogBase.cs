using CustomerService.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Collections;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class VideoCatalogBase: ComponentBase
    {
        [Inject]
        public IInboxService InboxService { get; set; }

        [Inject]
        public IVideoCatalogService VideoCatalogService { get; set; }

        [Inject]
        public IWatchlistService WatchlistService { get; set; }

        [Inject]
        public IAuthService AuthService { get; set; }

        [Inject]
        public IWebAssemblyHostEnvironment Environment { get; set; }

        private string selectedOption = "movie"; 
        private string selectedGenre = "";
        private List<Video> addedToWatchlist = new List<Video>();
        private string userId = "";
        public bool isAdmin = false;

        public VideoCatalogResult VideoCatalog { get; set; }
        public WatchlistResult Watchlist { get; set; } = new WatchlistResult();
        public GenresResult Genres { get; set; }

        public string selectedTitleId;

        public double selectedPrice;

        protected override async Task OnInitializedAsync()
        {
            Genres = await VideoCatalogService.GetGenres();

            AuthenticationResult authenticationResult = await AuthService.AuthenticateAdmin();
            if (!authenticationResult.Successful) return;
            isAdmin = authenticationResult.IsAdmin;

            userId = await AuthService.GetUserId();
            if (userId == null) return;

            WatchlistResult watchlistResult = await WatchlistService.GetTitleFromWatchlist(userId);
            addedToWatchlist = watchlistResult.Watchlist;
        }

        public async void OnTypeChange(string option)
        {
            if(selectedOption != option && !string.IsNullOrEmpty(selectedGenre))
            {
                VideoCatalog = await VideoCatalogService.GetVideos(option, selectedGenre);
                selectedOption = option;
                StateHasChanged();
            }
        }

        public async void OnGenreChange(ChangeEventArgs e)
        {
            string genre = e.Value.ToString();
            VideoCatalog = await VideoCatalogService.GetVideos(selectedOption, genre);
            selectedGenre = genre;
            StateHasChanged();
        }

        public async void OnAddToWatchlist(string titleId)
        {
            WatchlistModel watchlistModel = new WatchlistModel();
            watchlistModel.TitleId = titleId;
            watchlistModel.UserId = userId;

            Watchlist = await WatchlistService.AddTitleToWatchlist(watchlistModel);
            WatchlistResult watchlistResult = await WatchlistService.GetTitleFromWatchlist(userId);
            addedToWatchlist = watchlistResult.Watchlist;
            StateHasChanged();
        }

        public async void OnPublishUpcoming(string title)
        {
            UpcomingVideo upcomingVideo = new UpcomingVideo();
            upcomingVideo.Title = title;
            upcomingVideo.Genre = selectedGenre;

            var result = await InboxService.PublishUpcoming(upcomingVideo);
            if (!result.Successful) return;

            StateHasChanged();
        }

        public bool IsAddedToWatchlist(string titleId)
        {
            return addedToWatchlist.Any(video => video.Id == titleId);
        }

        public string GetImagePath()
        {
            string webRootPath = Environment.BaseAddress;
            return Path.Combine(webRootPath, "no-image.jpg");
        }

        public void SelectVideoForPayment(string titleId, double price)
        {
            selectedTitleId = titleId;
            selectedPrice = price;
        }
    }
}
