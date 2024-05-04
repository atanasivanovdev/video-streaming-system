using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PaymentService.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("titleId")]
        public string TitleId { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("cardNumber")]
        public string CardNumber { get; set; }

        [BsonElement("expirationCardDate")]
        public string ExpirationDate { get; set; }

        [BsonElement("paymentStatus")]
        public string PaymentStatus { get; set; }

        [BsonElement("cvc")]
        public string CVC { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

}
