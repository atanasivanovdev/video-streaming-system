using WatchlistService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RestSharp;
using VideoCatalogService.Models;
using Newtonsoft.Json;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using MongoDB.Bson;

namespace WatchlistService.Services
{
    public class UserWatchlistService
    {
        private readonly IMongoCollection<Watchlist> _watchlistCollection;
        private readonly RestClient _client;
        private readonly string _projectId;
        private readonly string _topicId;
        private readonly string _subscriptionId;

        public UserWatchlistService(IOptions<DatabaseSettings> databaseSettings, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _watchlistCollection = mongoDatabase.GetCollection<Watchlist>(databaseSettings.Value.WatchlistCollectionName);

            _client = new RestClient(configuration["VideoCatalogAPI:BaseUrl"]);
            _projectId = configuration["Authentication:Google:ProjectId"];
            _subscriptionId = configuration["Authentication:Google:SubscriptionId"];
            _topicId = configuration["Authentication:Google:TopicId"];
        }

        public async Task AddTitleToWatchlist(string userId, string titleId)
        {
            var update = Builders<Watchlist>.Update.AddToSet(wl => wl.Titles, titleId);
            await _watchlistCollection.UpdateOneAsync(wl => wl.UserId == userId, update, new UpdateOptions { IsUpsert = true });
        }

        public async Task<List<string>> FindUsersByGenreAsync(string genre)
        {
            var watchlists = await _watchlistCollection.Find(new BsonDocument()).ToListAsync();
            List<string> userIds = new List<string>();

            foreach (var watchlist in watchlists)
            {
                var ids = String.Join(",", watchlist.Titles.Select(t => t));
                var request = new RestRequest($"api/video/titles/{ids}", Method.Get);

                var response = await _client.GetAsync(request);
                if (response.IsSuccessful)
                {
                    var videos = JsonConvert.DeserializeObject<List<Video>>(response.Content);

                    if (videos.Any(v => v.Genres.Contains(genre)))
                    {
                        userIds.Add(watchlist.UserId);
                    }
                }
            }
            return userIds;
        }

        public async Task<List<Video>> GetTitlesFromWatchlist(string userId)
        {
            var watchlist = await _watchlistCollection.Find<Watchlist>(wl => wl.UserId == userId).FirstOrDefaultAsync();
            var videos = new List<Video>();

            if (watchlist?.Titles == null || !watchlist.Titles.Any())
            {
                return videos;
            }

            var ids = String.Join(",", watchlist.Titles);
            var request = new RestRequest($"api/video/titles/{ids}", Method.Get);
            var response = await _client.GetAsync(request);
            if (!response.IsSuccessful)
            {
                return videos;
            }
            
            videos = JsonConvert.DeserializeObject<List<Video>>(response.Content);
            return videos;
        }

        public async Task RemoveTitleFromWatchlist(string userId, string titleId)
        {
            var update = Builders<Watchlist>.Update.Pull(wl => wl.Titles, titleId);
            await _watchlistCollection.UpdateOneAsync(wl => wl.UserId == userId, update);
        }

        public async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancel) =>
            {
                string textData = message.Data.ToStringUtf8();
                Console.WriteLine($"Message received: {textData}");

                message.Attributes.TryGetValue("Genre", out string genre);

                if (!string.IsNullOrEmpty(genre))
                {
                    try
                    {
                        List<string> users = await FindUsersByGenreAsync(genre);
                        var newMessage = $"{textData} We noticed that this video has the same genre as other videos in your watchlist..";

                        foreach (string userId in users)
                        {
                            await PublishPostAsync(userId, newMessage);
                        }

                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        return SubscriberClient.Reply.Nack;
                    }
                }
                else
                {
                    Console.WriteLine("Necessary attributes not found or incomplete data.");
                    return SubscriberClient.Reply.Nack;
                }
            });

            Console.WriteLine("Listening for messages...");
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private async Task PublishPostAsync(string userId, string newMessage)
        {
            TopicName topicName = TopicName.FromProjectTopic(_projectId, _topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            try
            {
                PubsubMessage message = new PubsubMessage
                {
                    Data = ByteString.CopyFromUtf8(newMessage),
                    Attributes =
                    {
                        { "UserId", userId }
                    }
                };

                string messageId = await publisher.PublishAsync(message);
                Console.WriteLine($"Message {messageId} published.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred when publishing the message: {ex.Message}");
            }
            finally
            {
                await publisher.ShutdownAsync(TimeSpan.FromSeconds(15));
            }
        }
    }
}
