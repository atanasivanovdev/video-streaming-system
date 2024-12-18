﻿namespace VideoCatalogService.Models
{
	public class Video
	{
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public int ReleaseYear { get; set; }
        public List<string> Genres { get; set; }
    }
}
