using SwipeFeast.API.Models;
using SwipeFeast.API.Services.Exceptions;
using SwipeFeast.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeFeast.Testing
{
	[TestClass]
	public	class GoogleServiceUnitTest
	{
		[TestMethod]
		public async Task TestGetRestaurantsFromGoogle_Unsuccessful_WhenFilterInvalid_NoFilter()
		{
			// Zurich City
			double latitue = 47.376837828576484;
			double longitude = 8.542468126188002;
			int radius = 10000;
			List<Filter> filters = new List<Filter>();
			filters.ForEach(f => f.Active = true);

			IGoogleService googleService = new GoogleService();
			await Assert.ThrowsExceptionAsync<FilterInvalidException>(async () => await googleService.GetRestaurantsFromGoogle(latitue, longitude, radius, filters));
		}

		[TestMethod]
		public async Task TestGetRestaurantsFromGoogle_Unsuccessful_WhenFilterInvalid_InvalidFilter()
		{
			// Zurich City
			double latitue = 47.376837828576484;
			double longitude = 8.542468126188002;
			int radius = 10000;
			List<Filter> filters = new List<Filter> { new Filter { Id = "Invalid_Filter", Active = true} };
			filters.ForEach(f => f.Active = true);

			IGoogleService googleService = new GoogleService();
			await Assert.ThrowsExceptionAsync<FilterInvalidException>(async () => await googleService.GetRestaurantsFromGoogle(latitue, longitude, radius, filters));
		}
	}
}
