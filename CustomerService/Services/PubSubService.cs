using CustomerService.Models;
using Google.Cloud.PubSub.V1;

namespace CustomerService.Services
{
    public class PubSubService
    {
        private readonly string _subscriptionId;
        private readonly string _projectId;
        private InboxService _inboxService;

        public PubSubService(IConfiguration configuration, InboxService inboxService)
        {
            _subscriptionId = configuration["Authentication:Google:SubscriptionId"];
            _projectId = configuration["Authentication:Google:ProjectId"];
            _inboxService = inboxService;
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
                        MessageDTO messageDTO = new MessageDTO();
                        messageDTO.UserId = userId;
                        messageDTO.Content = textData;
                        await _inboxService.CreateMessage(messageDTO);
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
