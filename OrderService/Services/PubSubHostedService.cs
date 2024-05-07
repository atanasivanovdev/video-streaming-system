using Google.Cloud.PubSub.V1;

namespace OrderService.Services
{
    public class PubSubHostedService : BackgroundService
    {
        private readonly OrderManagementService _orderService;

        public PubSubHostedService(OrderManagementService orderService)
        {
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var taskPayments = Task.Run(() => SubscriptionPayments(stoppingToken), stoppingToken);
            var taskUpcoming = Task.Run(() => SubscriptionUpcoming(stoppingToken), stoppingToken);

            await Task.WhenAll(taskPayments, taskUpcoming);
        }

        private async Task SubscriptionPayments(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _orderService.SubscribePaymentsAsync(stoppingToken);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private async Task SubscriptionUpcoming(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _orderService.SubscribeUpcomingAsync(stoppingToken);
                    await Task.Delay(1000, stoppingToken); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");

                }
            }
        }
    }
}
