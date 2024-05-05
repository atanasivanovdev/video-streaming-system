using Google.Cloud.PubSub.V1;

namespace OrderService.Services
{
    public class PubSubService
    {
        private readonly string _subscriptionId;
        private readonly string _projectId;
        private OrderManagementService _orderService;

        public PubSubService(IConfiguration configuration, OrderManagementService orderService)
        {
            _subscriptionId = configuration["Authentication:Google:SubscriptionId"];
            _projectId = configuration["Authentication:Google:ProjectId"];
            _orderService = orderService;
        }

        public async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancel) =>
            {
                string textData = message.Data.ToStringUtf8();
                Console.WriteLine($"Message received: {textData}");

                if (message.Attributes.TryGetValue("PaymentId", out string paymentId))
                {
                    try
                    {
                        await _orderService.CreateOrder(paymentId);
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
