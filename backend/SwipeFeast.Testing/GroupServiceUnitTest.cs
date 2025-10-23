using SwipeFeast.API.Models;
using SwipeFeast.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using SwipeFeast.API.Services.Exceptions;

namespace SwipeFeast.Testing
{
	[TestClass]
	public class GroupServiceUnitTest
	{
		[TestMethod]
		public async Task TestCreateGroup_Successful()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).ReturnsAsync(new List<Restaurant> { new Restaurant { Id = "TestRestaurant", Name = "TestRestaurant", PhotoUrls = new List<string> { "" } } });

			var exampleDefaultFilter = GoogleService.GetDefaultFilters().First();
			exampleDefaultFilter.Active = true;

			GroupCreationDto groupDto = new GroupCreationDto
			{
				Name = "TestGroup",
				Filters = new List<Filter> { exampleDefaultFilter },
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			var groupId = await groupService.CreateGroup(groupDto);

			Assert.IsFalse(Guid.Empty == groupId);
			Assert.IsTrue(groupService.IsGroupActive(groupId));
		}

		[TestMethod]
		public async Task TestCreateGroup_Unsuccessful_WhenFiltersInvalid()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).Throws(new FilterInvalidException());

			Filter invalidFilter = new Filter { Id = "Invalid_Filter", Active = true };
			GroupCreationDto groupDto = new GroupCreationDto
			{
				Name = "TestGroup",
				Filters = new List<Filter> { invalidFilter },
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			await Assert.ThrowsExceptionAsync<FilterInvalidException>(() => groupService.CreateGroup(groupDto));
		}

		[TestMethod]
		public async Task TestCreateGroup_Unsuccessful_WhenNoRestaurantsFound()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).Throws(new NoRestaurantsFoundException());
			GroupCreationDto groupDto = new GroupCreationDto
			{
				Name = "TestGroup",
				Filters = new List<Filter> { new Filter { Id = "TestFilter", Active = true } },
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			await Assert.ThrowsExceptionAsync<NoRestaurantsFoundException>(() => groupService.CreateGroup(groupDto));
		}

		[TestMethod]
		public async Task TestJoinGroup_Successful()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).ReturnsAsync(new List<Restaurant> { new Restaurant { Id = "TestRestaurant", Name = "TestRestaurant", PhotoUrls = new List<string> { "" } } });

			var exampleDefaultFilter = GoogleService.GetDefaultFilters().First();
			exampleDefaultFilter.Active = true;

			GroupCreationDto groupDto = new GroupCreationDto
			{
				Name = "TestGroup",
				Filters = new List<Filter> { exampleDefaultFilter },
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			var groupId = await groupService.CreateGroup(groupDto);
			var joinCode = groupService.GetGroup(groupId)!.JoinCode;

			var memberId = Guid.NewGuid();

			groupService.AddMemberToGroup(memberId, joinCode);

			Assert.IsTrue(groupService.IsMemberInGroup(groupId, memberId));
		}

		[TestMethod]
		public void TestJoinGroup_Unsuccessful_WhenGroupNotFound()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			var memberId = Guid.NewGuid();
			var invalidJoinCode = 10000000;

			Assert.ThrowsException<GroupNotFoundException>(() => groupService.AddMemberToGroup(memberId, invalidJoinCode));
		}


		[TestMethod]
		public void TestRejoinGroup_Unsuccesful()
		{
			var memberId = Guid.NewGuid();
			// TODO: finish this test, likely need to add function in GroupService to check if member is already in group
			//_groupService.AddMemberToGroup(memberId, _testGroupId);
			//_groupService.AddMemberToGroup(memberId, _testGroupId);

			//Assert.IsTrue(_groupService.IsMemberInGroup(_testGroupId, memberId));
		}

		[TestMethod]
		public async Task TestLeaveGroup_Successful()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).ReturnsAsync(new List<Restaurant> { new Restaurant { Id = "TestRestaurant", Name = "TestRestaurant", PhotoUrls = new List<string> { "" } } });

			var exampleDefaultFilter = GoogleService.GetDefaultFilters().First();
			exampleDefaultFilter.Active = true;

			GroupCreationDto groupDto = new GroupCreationDto
			{
				Name = "TestGroup",
				Filters = new List<Filter> { exampleDefaultFilter },
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			var groupId = await groupService.CreateGroup(groupDto);
			var joinCode = groupService.GetGroup(groupId)!.JoinCode;

			var memberId = Guid.NewGuid();

			groupService.AddMemberToGroup(memberId, joinCode);

			groupService.RemoveMemberFromGroup(groupId, memberId);

			Assert.IsFalse(groupService.IsMemberInGroup(groupId, memberId));
		}

		[TestMethod]
		public void TestLeaveGroup_Unsuccessful_WhenGroupNotFound()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			var memberId = Guid.NewGuid();
			var invalidGroupId = Guid.NewGuid();

			Assert.ThrowsException<GroupNotFoundException>(() => groupService.RemoveMemberFromGroup(invalidGroupId, memberId));
		}

		[TestMethod]
		public async Task TestLeaveGroup_Unsuccessful_WhenMemberNotFound()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).ReturnsAsync(new List<Restaurant> { new Restaurant { Id = "TestRestaurant", Name = "TestRestaurant", PhotoUrls = new List<string> { "" } } });

			var exampleDefaultFilter = GoogleService.GetDefaultFilters().First();
			exampleDefaultFilter.Active = true;

			GroupCreationDto groupDto = new GroupCreationDto
			{
				Name = "TestGroup",
				Filters = new List<Filter> { exampleDefaultFilter },
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			var groupId = await groupService.CreateGroup(groupDto);

			var invalidMemberId = Guid.NewGuid();

			Assert.ThrowsException<MemberNotFoundException>(() => groupService.RemoveMemberFromGroup(groupId, invalidMemberId));
		}

		[TestMethod]
		public async Task TestGetFilters_Successful()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);

			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).ReturnsAsync(new List<Restaurant> { new Restaurant { Id = "TestRestaurant", Name = "TestRestaurant", PhotoUrls = new List<string> { "" } } });

			var exampleFilters = groupService.GetDefaultFilters().Take(3).ToList();
			exampleFilters[0].Active = true;
			exampleFilters[1].Active = true;
			exampleFilters[2].Active = false;

			var memberId = Guid.NewGuid();
			var group = new GroupExistingDto
			{
				Name = "TestGroup",
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000,
				Filters = exampleFilters
			};

			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).ReturnsAsync(new List<Restaurant> { new Restaurant { Id = "TestRestaurant", Name = "TestRestaurant", PhotoUrls = new List<string> { "" } } });
			var groupId = await groupService.CreateGroup(group);

			var receivedFilters = groupService.GetFilters(groupId);
			Assert.AreEqual(exampleFilters.Count - 1, receivedFilters.Count, "The number of filters does not match.");
			for (int i = 0; i < exampleFilters.Count; i++)
			{
				if (exampleFilters[i].Active)
				{
					Assert.AreEqual(exampleFilters[i].Id, receivedFilters[i].Id, $"Filter ID mismatch at index {i}");
					Assert.AreEqual(exampleFilters[i].Active, receivedFilters[i].Active, $"Filter Active state mismatch at index {i}");
				}
			}
		}

		[TestMethod]
		public void TestGetFilters_Unsuccessful_WhenGroupNotFound()
		{
			IGroupService groupService = new GroupService(new Mock<IGoogleService>().Object);

			Assert.ThrowsException<GroupNotFoundException>(() => groupService.GetFilters(Guid.NewGuid()));
		}

		[TestMethod]
		public async Task TestSetFilters_Successful()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
 			IGroupService groupService = new GroupService(mockGoogleService.Object);
			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).ReturnsAsync(new List<Restaurant> { new Restaurant { Id = "TestRestaurant", Name = "TestRestaurant", PhotoUrls = new List<string> { "" } } });

			var filters = GoogleService.GetDefaultFilters().Take(5).ToList();
			filters.ForEach(f => f.Active = true);
			var group = new GroupExistingDto
			{
				Name = "TestGroup",
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000,
				Filters = filters
			};

			var groupId = await groupService.CreateGroup(group);
			var joinCode = groupService.GetGroup(groupId)!.JoinCode;

			var memberId = Guid.NewGuid();
			// TODO: Check here again for if the feature with admin rights for members has been implemented.
			groupService.AddMemberToGroup(memberId, joinCode);

			var newFilters = filters.Take(3).ToList();
			newFilters.ForEach(f => f.Active = true);

			await groupService.SetFilters(groupId, memberId, newFilters);

			var receivedFilters = groupService.GetFilters(groupId);
			Assert.AreEqual(newFilters.Count, receivedFilters.Count, "The number of filters does not match.");
			for (int i = 0; i < newFilters.Count; i++)
			{
				Assert.AreEqual(newFilters[i].Id, receivedFilters[i].Id, $"Filter ID mismatch at index {i}");
				Assert.AreEqual(newFilters[i].Active, receivedFilters[i].Active, $"Filter Active state mismatch at index {i}");
			}
		}

		[TestMethod]
		public async Task TestSetFilters_Unsuccessful_WhenGroupNotFound()
		{
			IGroupService groupService = new GroupService(new Mock<IGoogleService>().Object);

			await Assert.ThrowsExceptionAsync<GroupNotFoundException>(() => groupService.SetFilters(Guid.NewGuid(), Guid.NewGuid(), new List<Filter>()));
		}

		[TestMethod]
		public async Task TestSetFilters_Unsuccessful_WhenMemberNotFound()
		{
			Mock<IGoogleService> mockGoogleService = new Mock<IGoogleService>();
			IGroupService groupService = new GroupService(mockGoogleService.Object);
			mockGoogleService.Setup(s => s.GetRestaurantsFromGoogle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<List<Filter>>())).ReturnsAsync(new List<Restaurant> { new Restaurant { Id = "TestRestaurant", Name = "TestRestaurant", PhotoUrls = new List<string> { "" } } });
			var filter = GoogleService.GetDefaultFilters().First();

			var group = new GroupExistingDto
			{
				Name = "TestGroup",
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000,
				Filters = new List<Filter> { filter }
			};

			var invalidMemberId = Guid.NewGuid();

			var groupId = await groupService.CreateGroup(group);

			await Assert.ThrowsExceptionAsync<MemberNotFoundException>(() => groupService.SetFilters(groupId, invalidMemberId, new List<Filter>()));
		}
	}
}