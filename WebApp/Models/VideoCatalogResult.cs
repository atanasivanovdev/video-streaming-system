using WebApp.Models;

namespace WebApp.Models
{
	public class VideoCatalogResult
	{
		public bool Successful { get; set; }
		public string Error { get; set; }

		public List<Video> VideoCatalog { get; set; }
	}
}
