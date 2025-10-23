using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SwipeFeast.API.Controllers;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services;
using SwipeFeast.API.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeFeast.Testing
{
	[TestClass]
	public class GoogleServiceSystemTest
	{
		[TestMethod]
		public async Task TestGetRestaurantsFromGoogle_Successful()
		{
			// Zurich City
			double latitue = 47.376837828576484;
			double longitude = 8.542468126188002;
			int radius = 10000;
			List<Filter> filters = GoogleService.GetDefaultFilters().Take(5).ToList();
			filters.ForEach(f => f.Active = true);

			IGoogleService googleService = new GoogleService();
			List<Restaurant> restaurants = await googleService.GetRestaurantsFromGoogle(latitue, longitude, radius, filters);
			Assert.IsNotNull(restaurants);
			Assert.IsTrue(restaurants.Count > 0);
			Assert.IsTrue(restaurants.All(r => r.Name != null));
		}

		[TestMethod]
		public async Task TestGetRestaurantsFromGoogle_Unsuccessful_WhenNoRestaurantsFound()
		{
			// North Atlantic Ocean
			double latitue = 43.66649317187634;
			double longitude = -36.85954405288817;
			int radius = 10000;

			List<Filter> filters = GoogleService.GetDefaultFilters().Take(5).ToList();
			filters.ForEach(f => f.Active = false);

			IGoogleService googleService = new GoogleService();
			var restaurants = await googleService.GetRestaurantsFromGoogle(latitue, longitude, radius, filters);

			Assert.IsNotNull(restaurants);
			Assert.IsTrue(restaurants.Count == 0);
		}
	}
}
