using System.Net.Http;
using System.Net.Http.Json;
using WebApp.Models;

namespace WebApp.Services
{
	public class VideoCatalogService : IVideoCatalogService
	{
		private readonly HttpClient _httpClient;

        public VideoCatalogService(HttpClient httpClient)
        {
			_httpClient = httpClient;
		}

        public async Task<GenresResult> GetGenres()
		{
			var result = await _httpClient.GetAsync("gateway/video/genres");

			GenresResult genresResult = new GenresResult();

			genresResult.Successful = result.IsSuccessStatusCode;

			if (!genresResult.Successful)
			{
				var error = await result.Content.ReadAsStringAsync();
				genresResult.Error = error;
				return genresResult;
			}

			genresResult.Genres = await result.Content.ReadFromJsonAsync<List<string>>();

			return genresResult;
		}

		public async Task<VideoCatalogResult> GetVideos(string titleType, string genre)
		{
			string url = $"gateway/video/titles?titleType={Uri.EscapeDataString(titleType)}&genre={Uri.EscapeDataString(genre)}";
			var result = await _httpClient.GetAsync(url);

			VideoCatalogResult videoCatalogResult = new VideoCatalogResult();

			videoCatalogResult.Successful = result.IsSuccessStatusCode;

			if (!videoCatalogResult.Successful)
			{
				var error = await result.Content.ReadAsStringAsync();
				videoCatalogResult.Error = error;
				return videoCatalogResult;
			}

			videoCatalogResult.VideoCatalog = await result.Content.ReadFromJsonAsync<List<Video>>();

			return videoCatalogResult;
		}
	}
}
