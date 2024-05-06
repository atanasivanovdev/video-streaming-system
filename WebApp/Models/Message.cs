namespace WebApp.Models
{
    public class Message
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
