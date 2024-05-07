using CustomerService.Models;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CustomerService.Services
{
    public class InboxService
    {
        private readonly IMongoCollection<Message> _inboxCollection;
        private readonly string _projectId;
        private readonly string _subscriptionId;
        private readonly string _topicId;

        public InboxService(IOptions<DatabaseSettings> databaseSettings, IConfiguration configuration)
        {
            _projectId = configuration["Authentication:Google:ProjectId"];
            _subscriptionId = configuration["Authentication:Google:SubscriptionId"];
            _topicId = configuration["Authentication:Google:TopicId"];

            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _inboxCollection = mongoDatabase.GetCollection<Message>(databaseSettings.Value.InboxCollectionName);
        }

        public async Task CreateMessage(MessageDTO messageDTO)
        {
            Message message = new Message
            {
                UserId = messageDTO.UserId,
                Content = messageDTO.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _inboxCollection.InsertOneAsync(message);
        }

        public async Task<List<Message>> GetMessages(string userId)
        {
            return await _inboxCollection
                .Find(message => message.UserId == userId)
                .Sort(Builders<Message>.Sort.Descending(message => message.CreatedAt))
                .ToListAsync();
        }

        public async Task PublishUpcomingAsync(UpcomingVideo upcomingVideo)
        {
            TopicName topicName = TopicName.FromProjectTopic(_projectId, _topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            try
            {
                PubsubMessage message = new PubsubMessage
                {
                    Data = ByteString.CopyFromUtf8($"A new video with Title: {upcomingVideo.Title} is about to be released!"),
                    Attributes =
                    {
                        { "Genre", upcomingVideo.Genre },
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

        public async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancel) =>
            {
                string textData = message.Data.ToStringUtf8();
                Console.WriteLine($"Message received: {textData}");

                if (message.Attributes.TryGetValue("UserId", out string userId))
                {
                    try
                    {
                        MessageDTO messageDTO = new MessageDTO
                        {
                            UserId = userId,
                            Content = textData
                        };
                        await CreateMessage(messageDTO);
                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
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
