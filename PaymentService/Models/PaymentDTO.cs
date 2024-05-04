using MongoDB.Bson.Serialization.Attributes;

namespace PaymentService.Models
{
    public class PaymentDTO
    {
        public string TitleId { get; set; }

        public string UserId { get; set; }

        public double Amount { get; set; }

        public string CardNumber { get; set; }

        public string ExpirationDate { get; set; }

        public string CVC { get; set; }
    }
}
