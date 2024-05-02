using WebApp.Models;

namespace WebApp.Services
{
	public interface IVideoCatalogService
	{
		Task<GenresResult> GetGenres();
		Task<VideoCatalogResult> GetVideos(string titleType, string genre);
	}
}
