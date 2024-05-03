namespace WebApp.Models
{
    public class WatchlistResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }

        public List<Video> Watchlist { get; set; }
    }
}
