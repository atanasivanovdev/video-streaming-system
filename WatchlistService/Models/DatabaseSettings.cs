namespace WatchlistService.Models
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string WatchlistCollectionName { get; set; } = null!;
    }
}
