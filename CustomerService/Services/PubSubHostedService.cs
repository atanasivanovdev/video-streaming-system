namespace CustomerService.Services
{
    public class PubSubHostedService : BackgroundService
    {
        private readonly PubSubService _pubSubService;

        public PubSubHostedService(PubSubService pubSubService)
        {
            _pubSubService = pubSubService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _pubSubService.SubscribeAsync(stoppingToken);
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
