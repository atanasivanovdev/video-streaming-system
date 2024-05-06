using CustomerService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace CustomerService.Services
{
    public class InboxService
    {
        private readonly IMongoCollection<Message> _inboxCollection;

        public InboxService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _inboxCollection = mongoDatabase.GetCollection<Message>(
                databaseSettings.Value.InboxCollectionName);
        }

        public async Task CreateMessage(MessageDTO messageDTO)
        {
            Message message = new Message();
            message.UserId = messageDTO.UserId;
            message.Content = messageDTO.Content;
            message.CreatedAt = DateTime.UtcNow;

            await _inboxCollection.InsertOneAsync(message);
        }

        public async Task<List<Message>> GetMessages(string userId)
        {
            return await _inboxCollection
                .Find(message => message.UserId == userId)
                .Sort(Builders<Message>.Sort.Descending(message => message.CreatedAt))
                .ToListAsync();
        }
    }
}
