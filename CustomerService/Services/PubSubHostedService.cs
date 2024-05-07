namespace CustomerService.Services
{
    public class PubSubHostedService : BackgroundService
    {
        private readonly InboxService _inboxService;

        public PubSubHostedService(InboxService inboxService)
        {
            _inboxService = inboxService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _inboxService.SubscribeAsync(stoppingToken);
                    await Task.Delay(10000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
