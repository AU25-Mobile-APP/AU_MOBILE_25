using SwipeFeast.API.Services.Exceptions;
using SwipeFeast.API.Models;

namespace SwipeFeast.API.Services
{
	/// <summary>
	/// Services that holds the necessary business logic of groups and how to interact with them / use date stored in them.
	/// </summary>
	public class GroupService : IGroupService
	{
		private List<Group> _activeGroups = new List<Group>();
		private IGoogleService _googleService;
		private Timer _timer;

		/// <summary>
		/// Constructor for the group service. Requires a Google Service to be injected.
		/// </summary>
		/// <param name="googleService">Google Service</param>
		/// <exception cref="ArgumentNullException"></exception>
		public GroupService(IGoogleService googleService)
		{
			_googleService = googleService ?? throw new ArgumentNullException(nameof(googleService));
			_timer = new Timer(CleanupExpiredGroups, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
		}

		/// <summary>
		/// Create a new group
		/// </summary>
		/// <param name="group">Group DTO</param>
		/// <exception cref="Exception"></exception>
		/// <exception cref="NoRestaurantsFoundException"></exception>
		/// <returns>A new group ID or empty GUID if it was not possible to get any restaurants from Google API.</returns>
		public async Task<Guid> CreateGroup(GroupCreationDto group)
		{
			var activeFilters = group.Filters.Where(f => f.Active).ToList();
			var restaurants = new List<Restaurant>();
			try
			{
				restaurants = await _googleService.GetRestaurantsFromGoogle(group.Longitude, group.Latitude, group.LocationRange, activeFilters);
			}
			catch (Exception)
			{
				throw;
			}

			if (restaurants == null || restaurants.Count == 0)
			{
				throw new NoRestaurantsFoundException();
			}

			var newGroup = new Group
            {
                Id = Guid.NewGuid(),
                Name = group.Name,
				JoinCode = CreateJoinCode(10000000, 99999999),
                Latitude = group.Latitude,
                Longitude = group.Longitude,
                LocationRange = group.LocationRange,
                Filters = activeFilters,
                ExpirationDate = DateTime.UtcNow.AddMinutes(300),
                Restaurants = restaurants
            };

			newGroup.Rankings = newGroup.Restaurants.Select(r => new Ranking { RestaurantId = r.Id, RestaurantName = r.Name, LikeCount = 0, Members = [] }).ToList();

            _activeGroups.Add(newGroup);
			return newGroup.Id;
		}

        /// <summary>
        /// Gives the filters of a group.
        /// </summary>
        /// <param name="groupId">Group ID</param>
        /// <returns>List of filters</returns>
        /// <exception cref="GroupNotFoundException"></exception>
        public List<Filter> GetFilters(Guid groupId)
		{
			Group? group = _activeGroups.FirstOrDefault(s => s.Id == groupId) ?? throw new GroupNotFoundException();
            return group.Filters;
		}

		/// <summary>
		/// Sets the filters of a group.
		/// </summary>
		/// <param name="groupId">Group ID</param>
		/// <param name="filters">List of filters to be set for active group.</param>
		/// <param name="memberId">Member ID</param>
		/// <exception cref="GroupNotFoundException"></exception>
		/// <exception cref="MemberNotFoundException"></exception>
		/// <exception cref="FilterInvalidException"></exception>
		/// <exception cref="NoRestaurantsFoundException"></exception>
		public async Task SetFilters(Guid groupId, Guid memberId, List<Filter> filters)
		{
            Group? group = _activeGroups.FirstOrDefault(s => s.Id == groupId);
			if (group == null)
			{
				throw new GroupNotFoundException();
			}

            if (!group.Members.Contains(memberId))
            {
				throw new MemberNotFoundException();
            }


            try
			{
				group.Filters = filters;

				var restaurants = await _googleService.GetRestaurantsFromGoogle(group.Longitude, group.Latitude, group.LocationRange, filters);
				group.Restaurants = restaurants;
				if (restaurants == null || restaurants.Count == 0)
				{
					throw new NoRestaurantsFoundException();
				}

				group.Rankings = group.Restaurants.Select(r => new Ranking { RestaurantId = r.Id, RestaurantName = r.Name, LikeCount = 0, Members = [] }).ToList();
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Increase like count for a restaurant that is part of an active group.
		/// </summary>
		/// <param name="groupdId">Group ID</param>
		/// <param name="memberId">Member ID</param>
		/// <param name="restaurantId">Restaurant ID</param>
		public void IncreaseLikeForRestaurant(Guid groupdId,Guid memberId, string restaurantId)
		{
			Group? group = _activeGroups.FirstOrDefault(s => s.Id == groupdId) ?? throw new GroupNotFoundException();
			if(!group.Members.Contains(memberId))
			{
				throw new MemberNotFoundException();
			}

            if (string.IsNullOrEmpty(restaurantId))
			{
				throw new Exception("Restaurant ID cannot be NULL.");
			}

			Ranking restaurantRanking = group.Rankings.FirstOrDefault(r => r.RestaurantId == restaurantId) ?? throw new RankingNotFoundException();
            
			if (!restaurantRanking.Members.Contains(memberId)) {
                restaurantRanking.Members.Add(memberId);
                restaurantRanking.LikeCount += 1;
            } 
        }

		/// <summary>
		/// Get the restaurants of a group.
		/// </summary>
		/// <param name="groupId">Group ID</param>
		/// <param name="memberId">Member ID</param>
		/// <returns>ist of restaurants of a specific group.</returns>
		/// <exception cref="GroupNotFoundException"></exception>
		/// <exception cref="MemberNotFoundException"></exception>
		public List<Restaurant> GetRestaurants(Guid groupId, Guid memberId)
		{
			Group? group = _activeGroups.FirstOrDefault(s => s.Id == groupId) ?? throw new GroupNotFoundException();

			if (!group.Members.Contains(memberId)) throw new MemberNotFoundException();

            return group.Restaurants;
		}

		/// <summary>
		/// Get a restaurant by its ID.
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="memberId"></param>
		/// <param name="restaurantId"></param>
		/// <returns></returns>
		/// <exception cref="GroupNotFoundException"></exception>
		/// <exception cref="MemberNotFoundException"></exception>
		/// <exception cref="RestaurantNotFoundException"></exception>
		public Restaurant GetRestaurant(Guid groupId, Guid memberId, string restaurantId)
		{
			Group? group = _activeGroups.FirstOrDefault(s => s.Id == groupId) ?? throw new GroupNotFoundException();

			if (!group.Members.Contains(memberId))
			{
                throw new MemberNotFoundException();
            }

			Restaurant? restaurant = group.Restaurants.FirstOrDefault(r => r.Id == restaurantId) ?? throw new RestaurantNotFoundException();

			return restaurant;
		}

		/// <summary>
		/// Adds a member to a group
		/// </summary>
		/// <param name="memberId"></param>
		/// <param name="joinCode"></param>
		/// <exception cref="GroupNotFoundException"></exception>
		public Guid AddMemberToGroup(Guid memberId, int joinCode)
		{
			var group = _activeGroups.FirstOrDefault(s => s.JoinCode == joinCode) ?? throw new GroupNotFoundException();
            if (!group.Members.Contains(memberId))
            {
                group.Members.Add(memberId);
            }

			return group.Id;
            
		}

		/// <summary>
		/// Removes a member from a group
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="memberId"></param>
		/// <exception cref="GroupNotFoundException"></exception>
		/// <exception cref="MemberNotFoundException"></exception>
		public void RemoveMemberFromGroup(Guid groupId, Guid memberId)
		{
			// TODO: check/test/define whether other member can call this aswell in theory.
			var group = _activeGroups.FirstOrDefault(r => r.Id == groupId);

			if (group == null)
			{
				throw new GroupNotFoundException();
			}

			if (!group.Members.Contains(memberId))
			{
				throw new MemberNotFoundException();
			}
			group.Members.Remove(memberId);
		}

		/// <summary>
		/// Get a group by its ID.
		/// </summary>
		/// <param name="groupId">Group ID</param>
		/// <returns></returns>
		/// <exception cref="GroupNotFoundException"></exception>
		public GroupExistingDto? GetGroup(Guid groupId)
		{
			Group? group = _activeGroups.FirstOrDefault(s => s.Id == groupId);
			if (group == null)
			{
				return null;
			}

			return new GroupExistingDto
			{
				Latitude = group.Latitude,
				Longitude = group.Longitude,
				LocationRange = group.LocationRange,
				Filters = group.Filters,
				ExpirationDate = group.ExpirationDate,
				Name = group.Name,
				JoinCode = group.JoinCode
			};
		}

		/// <summary>
		/// Get the default filters.
		/// </summary>
		/// <returns>List of default filters.</returns>
		public List<Filter> GetDefaultFilters()
		{
			return GoogleService.GetDefaultFilters();
		}

		/// <summary>
		/// Get the list of rankings for a group. The list is sorted by the like count in descending order.
		/// </summary>
		/// <param name="groupId">Group ID</param>
		/// <returns></returns>
		/// <exception cref="GroupNotFoundException"></exception>
		public List<Ranking> GetListOfRankings(Guid groupId)
		{
			List<Ranking> rankings = [];
			
			Group? group = _activeGroups.FirstOrDefault(s => s.Id == groupId) ?? throw new GroupNotFoundException();
            
			rankings = group.Rankings.OrderByDescending(r => r.LikeCount).ToList();

			return rankings;
        }

		/// <summary>
		/// Checks if a group is active.
		/// </summary>
		/// <param name="groupId">Group ID</param>
		/// <returns>True if group is active, false is group is not active.</returns>
		public bool IsGroupActive(Guid groupId)
		{
			return _activeGroups.Any(g => g.Id == groupId);
		}

		/// <summary>
		/// Checks if a group contains a specific member.
		/// </summary>
		/// <param name="groupId">Group ID</param>
		/// <param name="memberId">Member ID</param>
		/// <returns>True if Member is in group, false if Member ID is not in group.</returns>
		/// <exception cref="GroupNotFoundException"></exception>
		public bool IsMemberInGroup(Guid groupId, Guid memberId)
		{
			var group = _activeGroups.FirstOrDefault(r => r.Id == groupId);
			if (group == null)
			{
				throw new GroupNotFoundException();
			}

			return group.Members.Contains(memberId);
		}

		/// <summary>
		/// Removes expired groups.
		/// </summary>
		/// <param name="state"></param>
		private void CleanupExpiredGroups(object? state)
		{
			var now = DateTime.UtcNow;
			int removedCount = _activeGroups.RemoveAll(group => group.ExpirationDate <= now);
		}

		/// <summary>
		/// Creates a join code for a group.
		/// </summary>
		/// <param name="startValue"></param>
		/// <param name="endValue"></param>
		/// <returns></returns>
        private int CreateJoinCode(int startValue, int endValue)
        {
            var joinCode = 0;
            var isNotValid = true;
            do
            {
                joinCode = new Random().Next(startValue, endValue);
                _activeGroups.ForEach(g =>
                {
                    if (g.JoinCode == joinCode)
                    {
                        isNotValid = true;
                    }
                    else
                    {
                        isNotValid = false;
                    }
                });
            } while (isNotValid && _activeGroups.Count > 0);

            return joinCode;
        }
    }
}
