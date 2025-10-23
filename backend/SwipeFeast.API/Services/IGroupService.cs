using SwipeFeast.API.Models;

namespace SwipeFeast.API.Services
{
	public interface IGroupService
	{

		public Task<Guid> CreateGroup(GroupCreationDto group);

		public List<Filter> GetFilters(Guid groupId);

		public async Task SetFilters(Guid groupId, Guid memberId, List<Filter> filters) { }

		public void IncreaseLikeForRestaurant(Guid groupdId, Guid memberId, string restaurantId);

		public List<Restaurant> GetRestaurants(Guid groupId, Guid memberId);

		public Restaurant GetRestaurant(Guid groupId, Guid memberId, string restaurantId);

		public Guid AddMemberToGroup(Guid memberId, int joinCode);

		public void RemoveMemberFromGroup(Guid groupId, Guid memberId);

		public GroupExistingDto? GetGroup(Guid groupId);

		public List<Filter> GetDefaultFilters();

		public List<Ranking> GetListOfRankings(Guid groupId);

		public bool IsGroupActive(Guid groupId);

		public bool IsMemberInGroup(Guid groupId, Guid clientId);
	}
}
