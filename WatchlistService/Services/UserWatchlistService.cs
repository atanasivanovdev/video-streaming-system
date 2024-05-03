using WatchlistService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RestSharp;
using VideoCatalogService.Models;
using Newtonsoft.Json;

namespace WatchlistService.Services
{
    public class UserWatchlistService
    {
        private readonly IMongoCollection<Watchlist> _watchlistCollection;
        private readonly RestClient _client;

        public UserWatchlistService(IOptions<DatabaseSettings> databaseSettings, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _watchlistCollection = mongoDatabase.GetCollection<Watchlist>(
                databaseSettings.Value.WatchlistCollectionName);

            _client = new RestClient(configuration["VideoCatalogAPI:BaseUrl"]);
        }

        public async Task AddTitleToWatchlist(string userId, string titleId)
        {
            var update = Builders<Watchlist>.Update.AddToSet(wl => wl.Titles, titleId);
            await _watchlistCollection.UpdateOneAsync(wl => wl.UserId == userId, update, new UpdateOptions { IsUpsert = true });
        }

        public async Task<List<Video>> GetTitlesFromWatchlist(string userId)
        {
            var watchlist = await _watchlistCollection.Find<Watchlist>(wl => wl.UserId == userId).FirstOrDefaultAsync();
            var videos = new List<Video>();

            if (watchlist?.Titles == null || !watchlist.Titles.Any())
            {
                return videos;
            }

            foreach (var titleId in watchlist.Titles)
            {
                var request = new RestRequest($"api/video/titles/{titleId}", Method.Get);
                var response = await _client.GetAsync(request);
                if (response.IsSuccessful)
                {
                    var video = JsonConvert.DeserializeObject<Video>(response.Content);
                    if (video != null)
                    {
                        videos.Add(video);
                    }
                }
            }
            return videos;
        }

        public async Task RemoveTitleFromWatchlist(string userId, string titleId)
        {
            var update = Builders<Watchlist>.Update.Pull(wl => wl.Titles, titleId);
            await _watchlistCollection.UpdateOneAsync(wl => wl.UserId == userId, update);
        }
    }
}
