﻿using Google.Cloud.PubSub.V1;
using Google.Protobuf;
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
        private readonly string _projectId;
        private readonly string _subscriptionId;
        private readonly string _topicId;

        public OrderManagementService(IOptions<DatabaseSettings> databaseSettings, IConfiguration configuration)
        {
            // Database setup
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _ordersCollection = mongoDatabase.GetCollection<Order>(
                databaseSettings.Value.OrderCollectionName);

            // REST Clients
            _paymentClient = new RestClient(configuration["ExternalAPI:PaymentURL"]);
            _videoClient = new RestClient(configuration["ExternalAPI:VideoCatalogURL"]);

            // Pub/Sub Settings
            _subscriptionId = configuration["Authentication:Google:SubscriptionId"];
            _projectId = configuration["Authentication:Google:ProjectId"];
            _topicId = configuration["Authentication:Google:TopicId"];
        }

        private async Task PublishPostAsync(string userId, string title)
        {
            TopicName topicName = TopicName.FromProjectTopic(_projectId, _topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            try
            {
                PubsubMessage message = new PubsubMessage
                {
                    Data = ByteString.CopyFromUtf8($"Your order for the video '{title}' has been successfully completed!"),
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

        private async Task CreateOrder(string paymentId)
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
            await PublishPostAsync(order.UserId, order.Title);
        }

        public async Task<List<Order>> GetOrdersByUserId(string userId)
        { 
            return await _ordersCollection.Find(order => order.UserId == userId).ToListAsync();
        }

        public async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancel) =>
            {
                string textData = message.Data.ToStringUtf8();
                Console.WriteLine($"Message received: {textData}");

                if (message.Attributes.TryGetValue("PaymentId", out string paymentId))
                {
                    try
                    {
                        await CreateOrder(paymentId);
                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing order: {ex.Message}");
                        return SubscriberClient.Reply.Nack;
                    }
                }

                return SubscriberClient.Reply.Ack;
            });

            Console.WriteLine("Listening for messages...");
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }
}