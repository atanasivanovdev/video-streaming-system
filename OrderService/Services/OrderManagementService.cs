using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderService.Models;
using RestSharp;

namespace OrderService.Services
{
    public class OrderManagementService
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly RestClient _paymentClient;
        private readonly RestClient _videoClient;

        public OrderManagementService(IOptions<DatabaseSettings> databaseSettings, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _ordersCollection = mongoDatabase.GetCollection<Order>(
                databaseSettings.Value.OrderCollectionName);

            _paymentClient = new RestClient(configuration["ExternalAPI:PaymentURL"]);
            _videoClient = new RestClient(configuration["ExternalAPI:VideoCatalogURL"]);
        }

        public async Task CreateOrder(string paymentId)
        {
            Order order = new Order();

            var paymentRequest = new RestRequest($"api/payment/{paymentId}", Method.Get);
            var paymentResponse = await _paymentClient.GetAsync(paymentRequest);
            if (paymentResponse == null)
            {
                Console.WriteLine("Failed to retrieve payment details.");
                return;
            }

            var jsonPayment = JsonConvert.DeserializeObject<JObject>(paymentResponse.Content);
            order.PaymentId = paymentId;
            order.Amount = jsonPayment["amount"].Value<double>();
            order.CardExpiration = jsonPayment["expirationDate"].Value<string>();
            order.Status = jsonPayment["paymentStatus"].Value<string>();
            order.PlacedOn = DateTime.UtcNow;

            var cardNumber = jsonPayment["cardNumber"].Value<string>();
            order.LastCardDigits = cardNumber.Substring(cardNumber.Length - 4);

            order.UserId = jsonPayment["userId"].Value<string>();
            order.TitleId = jsonPayment["titleId"].Value<string>();

            var videoRequest = new RestRequest($"api/video/titles/{order.TitleId}", Method.Get);
            var videoResponse = await _videoClient.GetAsync(videoRequest);
            if (videoResponse == null)
            {
                Console.WriteLine("Failed to retrieve video details.");
                return;
            }

            var jsonVideo = JsonConvert.DeserializeObject<JObject>(videoResponse.Content);
            order.Title = jsonVideo["title"].Value<string>();
            order.ImageURL = jsonVideo["imageURL"].Value<string>();

            await _ordersCollection.InsertOneAsync(order);
        }

        public async Task<List<Order>> GetOrdersByUserId(string userId)
        { 
            return await _ordersCollection.Find(order => order.UserId == userId).ToListAsync();
        }
    }
}
