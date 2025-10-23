using SwipeFeast.API.Models;

namespace SwipeFeast.API.Services
{
	public interface IGoogleService
	{
		public Task<List<Restaurant>> GetRestaurantsFromGoogle(double longitude, double latitude, int locationRange, List<Filter> filters);
	}
}
