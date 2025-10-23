namespace SwipeFeast.API.Models
{
	/// <summary>
	/// Data model to represent a restaurant.
	/// </summary>
	public class Restaurant
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public float Rating { get; set; }
		public List<Filter> Suitable { get; set; }
		public string NavigationUrl { get; set; }
		public List<string> PhotoUrls { get; set; }
	}
}
