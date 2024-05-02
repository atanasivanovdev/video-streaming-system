using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class VideoCatalogBase: ComponentBase
    {
        [Inject]
        public IVideoCatalogService VideoCatalogService { get; set; }

        [Inject]
        public IWebAssemblyHostEnvironment Environment { get; set; }

        private string selectedOption = "movie"; 
        private string selectedGenre = "";

        public VideoCatalogResult VideoCatalog { get; set; }
        public GenresResult Genres { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Genres = await VideoCatalogService.GetGenres();
        }

        public async void OnTypeChange(string option)
        {
            if(selectedOption != option && !string.IsNullOrEmpty(selectedGenre))
            {
                VideoCatalog = await VideoCatalogService.GetVideos(selectedOption, selectedGenre);
                selectedOption = option;
            }
        }

        public async void OnGenreChange(ChangeEventArgs e)
        {
            string genre = e.Value.ToString();
            VideoCatalog = await VideoCatalogService.GetVideos(selectedOption, genre);
            selectedGenre = genre;
        }

        public string GetImagePath()
        {
            string webRootPath = Environment.BaseAddress;
            return Path.Combine(webRootPath, "no-image.jpg");
        }
    }
}
