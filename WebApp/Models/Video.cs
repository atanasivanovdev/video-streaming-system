namespace WebApp.Models
{
	public class Video
	{
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public int ReleaseYear { get; set; }
        public double Price { get; set; } = 9.99;
    }
}
