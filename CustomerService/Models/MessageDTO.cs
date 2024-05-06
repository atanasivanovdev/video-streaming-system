using MongoDB.Bson.Serialization.Attributes;

namespace CustomerService.Models
{
    public class MessageDTO
    {
        public string UserId { get; set; }
        public string Content { get; set; }
    }
}
