using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Text.Json;
using VideoCatalogService.Models;
using System;

namespace VideoCatalogService.Services
{
	public class VideoService : IVideoService
	{
		private readonly RestClient _client;

		public VideoService(IConfiguration configuration)
		{
			_client = new RestClient(configuration["VideoAPI:BaseUrl"]);
			_client.AddDefaultHeader("X-RapidAPI-Key", configuration["VideoAPI:ApiKey"]);
			_client.AddDefaultHeader("X-RapidAPI-Host", configuration["VideoAPI:Host"]);
		}
		public async Task<List<string>> GetGenres()
		{
			var request = new RestRequest("titles/utils/genres", Method.Get);
			var response = await _client.GetAsync(request);

			if (!response.IsSuccessful)
			{
				return null!;
			}

			if (string.IsNullOrEmpty(response.Content))
			{
				return new List<string>();
			}

			var jsonObject = JsonConvert.DeserializeObject<JObject>(response.Content);
			var elements = jsonObject["results"];
			var genres = elements.Select(element => element.Value<string>() ?? string.Empty)
							.Where(genre => !string.IsNullOrWhiteSpace(genre))
							.ToList();
			return genres;
		}

		public async Task<List<Video>> GetTitles(string titleType, string genre)
		{
			var request = new RestRequest("titles", Method.Get);
			request.AddParameter("genre", genre);
			request.AddParameter("titleType", titleType);

			var response = await _client.GetAsync(request);

			if (!response.IsSuccessful)
			{
				return null!;
			}

			if (string.IsNullOrEmpty(response.Content))
			{
				return new List<Video>();
			}

			var videos = ParseVideosFromJson(response.Content);
			return videos;
		}

		private List<Video> ParseVideosFromJson(string jsonContent)
		{
			var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonContent);
			var elements = jsonObject["results"];
			if (elements == null) return new List<Video>();

			List<Video> videos = new List<Video>();

			foreach (JToken element in elements)
			{
				var id = (string)element["id"];
				var title = element["titleText"].HasValues ? (string)element["titleText"]["text"] : "";
				var imageUrl = element["primaryImage"].HasValues ? (string)element["primaryImage"]["url"] : "";
				var releaseYear = element["releaseYear"].HasValues ? (int)element["releaseYear"]["year"] : 0; 

				if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(title))
				{
					continue;
				}

				Video video = new Video
				{
					Id = id,
					Title = title,
					ImageURL = imageUrl ?? string.Empty,
					ReleaseYear = releaseYear
				};

				videos.Add(video);
			}

			return videos;
		}

	}
}
