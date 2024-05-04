using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PaymentService.Models;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace PaymentService.Services
{
    public class PaymentOrderService
    {
        private readonly IMongoCollection<Payment> _paymentsCollection;
        private readonly PubSubService _pubSubService;

        public PaymentOrderService(IOptions<DatabaseSettings> databaseSettings, PubSubService pubSubService)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _paymentsCollection = mongoDatabase.GetCollection<Payment>(
                databaseSettings.Value.PaymentCollectionName);

            _pubSubService = pubSubService;
        }

        public async Task<PaymentDTO> ProcessPayment(PaymentDTO paymentDTO)
        {
            Payment payment = new Payment();
            payment.TitleId = paymentDTO.TitleId;
            payment.CardNumber = paymentDTO.CardNumber;
            payment.ExpirationDate = paymentDTO.ExpirationDate;
            payment.Amount = paymentDTO.Amount;
            payment.UserId = paymentDTO.UserId;
            payment.CVC = paymentDTO.CVC;

            payment.PaymentStatus = "Completed";
            payment.CreatedAt = DateTime.UtcNow;

            await _paymentsCollection.InsertOneAsync(payment);
            await _pubSubService.PublishPostAsync(payment.UserId, payment.TitleId, payment.Id);
            return paymentDTO;
        }

        public async Task<Payment> GetPaymentById(string paymentId)
        {
            return await _paymentsCollection.Find(p => p.Id == paymentId).FirstOrDefaultAsync();
        }
    }
}
