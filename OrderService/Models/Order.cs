using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OrderService.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("titleId")]
        public string TitleId { get; set; }

        [BsonElement("paymentId")]
        public string PaymentId { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("placedOn")]
        public DateTime PlacedOn { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("genres")]
        public List<string> Genres { get; set; }

        [BsonElement("imageUrl")]
        public string ImageURL { get; set; }

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("lastCardDigits")]
        public string LastCardDigits { get; set; }

        [BsonElement("cardExpiration")]
        public string CardExpiration { get; set; }
    }
}
