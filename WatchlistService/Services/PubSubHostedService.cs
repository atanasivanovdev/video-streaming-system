namespace WatchlistService.Services
{
    public class PubSubHostedService : BackgroundService
    {
        private readonly UserWatchlistService _watchlistService;

        public PubSubHostedService(UserWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _watchlistService.SubscribeAsync(stoppingToken);
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
