using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace PaymentService.Services
{
    public class PubSubService
    {
        private readonly string _topicId;
        private readonly string _projectId;
        
        public PubSubService(IConfiguration configuration)
        {
            _topicId = configuration["Authentication:Google:TopicId"];
            _projectId = configuration["Authentication:Google:ProjectId"];
        }
        public async Task PublishPostAsync(string paymentId)
        {
            TopicName topicName = TopicName.FromProjectTopic(_projectId, _topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            try
            {
                PubsubMessage message = new PubsubMessage
                {
                    Data = ByteString.CopyFromUtf8("The payment has been successful!"),
                    Attributes =
                    {
                        { "PaymentId", paymentId }
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
    }
}
