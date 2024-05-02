namespace WebApp.Models
{
	public class GenresResult
	{
		public bool Successful { get; set; }
		public string Error { get; set; }

        public List<string> Genres { get; set; }
    }
}
