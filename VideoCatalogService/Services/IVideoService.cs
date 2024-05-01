using VideoCatalogService.Models;

namespace VideoCatalogService.Services
{
	public interface IVideoService
	{
		Task<List<string>> GetGenres();
		Task<List<Video>> GetTitles(string titleType, string genre);
	}
}
