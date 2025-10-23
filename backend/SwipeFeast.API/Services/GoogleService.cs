using SwipeFeast.API.Services.Exceptions;
using SwipeFeast.API.Models;
using System.Text.Json;
using System.Net.Http;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace SwipeFeast.API.Services
{
    /// <summary>
    /// Class that holds the necessary business logic of Google API and how to interact with it.
    /// </summary>
    public class GoogleService : IGoogleService
    {
        private static readonly List<Filter> defaultFilters = new List<Filter>
            {
                new Filter { Id = "american_restaurant", Active = false },
                new Filter { Id = "bakery", Active = false },
                new Filter { Id = "bar", Active = false },
                new Filter { Id = "barbecue_restaurant", Active = false },
                new Filter { Id = "brazilian_restaurant", Active = false },
                new Filter { Id = "breakfast_restaurant", Active = false },
                new Filter { Id = "brunch_restaurant", Active = false },
                new Filter { Id = "cafe", Active = false },
                new Filter { Id = "chinese_restaurant", Active = false },
                new Filter { Id = "coffee_shop", Active = false },
                new Filter { Id = "fast_food_restaurant", Active = false },
                new Filter { Id = "french_restaurant", Active = false },
                new Filter { Id = "greek_restaurant", Active = false },
                new Filter { Id = "hamburger_restaurant", Active = false },
                new Filter { Id = "ice_cream_shop", Active = false },
                new Filter { Id = "indian_restaurant", Active = false },
                new Filter { Id = "indonesian_restaurant", Active = false },
                new Filter { Id = "italian_restaurant", Active = false },
                new Filter { Id = "japanese_restaurant", Active = false },
                new Filter { Id = "korean_restaurant", Active = false },
                new Filter { Id = "lebanese_restaurant", Active = false },
                new Filter { Id = "mediterranean_restaurant", Active = false },
                new Filter { Id = "mexican_restaurant", Active = false },
                new Filter { Id = "middle_eastern_restaurant", Active = false },
                new Filter { Id = "pizza_restaurant", Active = false },
                new Filter { Id = "ramen_restaurant", Active = false },
                new Filter { Id = "sandwich_shop", Active = false },
                new Filter { Id = "seafood_restaurant", Active = false },
                new Filter { Id = "spanish_restaurant", Active = false },
                new Filter { Id = "steak_house", Active = false },
                new Filter { Id = "sushi_restaurant", Active = false },
                new Filter { Id = "thai_restaurant", Active = false },
                new Filter { Id = "turkish_restaurant", Active = false },
                new Filter { Id = "vegan_restaurant", Active = false },
                new Filter { Id = "vegetarian_restaurant", Active = false },
                new Filter { Id = "vietnamese_restaurant", Active = false }
            };

        /// <summary>
        /// Getting restaurants from Google API.
        /// </summary>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="locationRange">Location range in meters.</param>
        /// <param name="filters">List of applied filters.</param>
        /// <returns>List of restaurants from around given location inside range and filtered by provided filters.</returns>
        /// <exception cref="Exception">Exception for when Google API Token is not defined.</exception>
        public async Task<List<Restaurant>> GetRestaurantsFromGoogle(double longitude, double latitude, int locationRange, List<Filter> filters)
        {
            if (!IsFilterListValid(filters))
            {
                throw new FilterInvalidException();
            }

            using (var client = new HttpClient())
            {
                string requestData = new GoogleRequestData(latitude, longitude, locationRange, filters).JsonString;
                var httpContent = new StringContent(requestData, Encoding.UTF8, "application/json");

                string googleApiToken;
                if (!TryGetGoogleApiToken(out googleApiToken))
                {
                    throw new Exception("Google API Token not set in user secrets.");
                }

                if (string.IsNullOrEmpty(googleApiToken) || googleApiToken == "SET_IN_USER_SECRET")
                {
                    throw new Exception("Google API Token not set in user secrets.");
                }

                httpContent.Headers.Add("X-Goog-Api-Key", googleApiToken);
                httpContent.Headers.Add("X-Goog-FieldMask", "places.displayName,places.types,places.id,places.rating,places.location,places.googleMapsUri,places.editorialSummary,places.photos");
                string httpResponse = "";
                try
                {
                    HttpResponseMessage httpResponseMessage = await client.PostAsync("https://places.googleapis.com/v1/places:searchNearby", httpContent);
                    if (!httpResponseMessage.IsSuccessStatusCode)
                    {
                        throw new Exception("Google API request failed.");
                    }
                    httpResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                }
                catch (Exception)
                {
                    throw;
                }

                GooglePlaces? googlePlaces = new GooglePlaces();
                try
                {
                    if (string.IsNullOrEmpty(httpResponse))
                    {
                        googlePlaces = null;
                    }
                    else
                    {
                        googlePlaces = JsonSerializer.Deserialize<GooglePlaces>(httpResponse);
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                if (googlePlaces != null)
                {
                    List<Restaurant> restaurants = new List<Restaurant>();
                    foreach (var googleRestaurant in googlePlaces.places)
                    {
                        restaurants.Add(new Restaurant
                        {
                            Id = googleRestaurant.id,
                            NavigationUrl = googleRestaurant.googleMapsUri,
                            Suitable = googleRestaurant.types.Select(s => new Filter { Id = s, Active = true }).ToList(),
                            Rating = googleRestaurant.rating,
                            Name = googleRestaurant.displayName.text,
                            Description = googleRestaurant.editorialSummary?.text ?? string.Empty,
                            PhotoUrls = await GetPhotoUrlFromGoogle(googleRestaurant.photos, 600, 600)
                        });
                    }
                    return restaurants;
                }
            }
            throw new Exception("Google API request failed.");
        }

        /// <summary>
        /// Get photo URLs from Google API.
        /// </summary>
        /// <param name="googlePhotoIds">List of photo Ids </param>
        /// <param name="maxHeight"></param>
        /// <param name="maxWidth"></param>
        /// <returns>List of photo URLs</returns>
        /// <exception cref="Exception"></exception>
        private static async Task<List<string>> GetPhotoUrlFromGoogle(List<GooglePhotoId> googlePhotoIds, int maxHeight, int maxWidth)
        {
			List<string> photoUrls = [];
			if (googlePhotoIds == null)
			{
                return photoUrls;
			}

			if (!TryGetGoogleApiToken(out string googleApiToken))
            {
                throw new Exception("Google API Token not set in user secrets.");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    for (int i = 0; i < googlePhotoIds.Count && i < 2; i++)
                    {
                        Uri photoUri = new($"https://places.googleapis.com/v1/{googlePhotoIds[i].name}/media?maxHeightPx={maxHeight}&maxWidthPx={maxWidth}&key={googleApiToken}");
                        HttpResponseMessage httpResponseMessage = await client.GetAsync(photoUri);

                        if (httpResponseMessage is not null)
                        {
                            if (httpResponseMessage.RequestMessage is not null)
                            {
                                if (httpResponseMessage.RequestMessage.RequestUri is not null)
                                {
                                    photoUrls.Add(httpResponseMessage.RequestMessage.RequestUri.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return photoUrls;
        }

        /// <summary>
        /// Get default filters for the Google API request.
        /// Taken from Google Places API: https://developers.google.com/maps/documentation/places/web-service/place-types#food-and-drink
        /// </summary>
        /// <returns></returns>
        public static List<Filter> GetDefaultFilters()
        {
            return defaultFilters;
        }

        /// <summary>
        /// Check if a list of filters is valid.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        private static bool IsFilterListValid(List<Filter> filters)
        {
            if (filters == null || filters.Count == 0)
            {
                return false;
            }
            foreach (var filter in filters)
            {
                if (!defaultFilters.Any(f => f.Id == filter.Id))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Try to get the Google API token from user secrets.
        /// </summary>
        /// <param name="googleApiToken">Variable for the Google Api Token to be stored in if successful.</param>
        /// <returns>True if getting API Token was successful, false if it was not.</returns>
        private static bool TryGetGoogleApiToken(out string googleApiToken)
        {
            IConfiguration _config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string setInUserSecretPlaceholder = "SET_IN_USER_SECRET";
            googleApiToken = _config.GetValue<string>("GoogleApiToken", setInUserSecretPlaceholder);
            return !string.IsNullOrEmpty(googleApiToken) && googleApiToken != setInUserSecretPlaceholder;
        }

    }


    class GoogleRequestData
    {
        public string JsonString { get; private set; }

        public GoogleRequestData(double latitude, double longitude, int locationRange, List<Filter> filters)
        {
            var stringBuilder = new StringBuilder();
            foreach (var filter in filters)
            {
                stringBuilder.Append('"');
                stringBuilder.Append(filter.Id);
                stringBuilder.Append('"');
                stringBuilder.Append(",");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            // JSON string for Google API request as defined in Google API documentation.
            var requestString = $@"
				{{
					""includedTypes"": [
						{stringBuilder.ToString()}
					],
					""maxResultCount"": 10,
                    ""languageCode"": ""de"",
					""locationRestriction"": {{
					""circle"": {{
						""center"": {{
							""latitude"": {latitude},
							""longitude"": {longitude}
							}},
							""radius"": {locationRange}
						}}
					}},
				}}";

            JsonString = requestString;
        }
    }

}
