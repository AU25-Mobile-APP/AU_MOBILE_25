using Microsoft.AspNetCore.Mvc;
using Moq;
using SwipeFeast.API.Controllers;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services.Exceptions;
using SwipeFeast.API.Services;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace SwipeFeast.Testing
{
	[TestClass]
	public class GroupControllerIntegrationTest
	{
		private Mock<IGroupService> _mockGroupService;
		private Mock<ILogger<GroupController>> _mockLogger;
		private Mock<IResponseCookies> _mockCookies;

		private Mock<HttpContext> _mockHttpContext;
		private Mock<HttpResponse> _mockHttpResponse;

		private GroupController _controller;

		[TestInitialize]
		public void Setup()
		{
			_mockGroupService = new Mock<IGroupService>();
			_mockLogger = new Mock<ILogger<GroupController>>();
			_mockCookies = new Mock<IResponseCookies>();

			_mockHttpResponse = new Mock<HttpResponse>();
			_mockHttpResponse.SetupGet(r => r.Cookies).Returns(_mockCookies.Object);

			_mockHttpContext = new Mock<HttpContext>();
			_mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);

			_controller = new GroupController(_mockGroupService.Object, _mockLogger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = _mockHttpContext.Object
				}
			};
		}

		[TestMethod]
		public void TestGetGroup_ReturnsOk_WhenGroupIsFound()
		{
			var groupId = Guid.NewGuid();
			var exampleDefaultFilter = GoogleService.GetDefaultFilters().First();
			var expectedGroup = new GroupExistingDto
			{
				Name = "TestGroup",
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000,
				Filters = new List<Filter> { exampleDefaultFilter }
			};

			_mockGroupService.Setup(service => service.GetGroup(groupId)).Returns(expectedGroup);

			var result = _controller.GetGroup(groupId) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			Assert.AreEqual(expectedGroup, result.Value);
		}

		[TestMethod]
		public void TestGetGroup_ReturnsNotFound_WhenGroupIsNotFound()
		{
			var groupId = Guid.NewGuid();
			_mockGroupService.Setup(service => service.GetGroup(groupId)).Throws(new GroupNotFoundException());

			var result = _controller.GetGroup(groupId) as NotFoundObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(404, result.StatusCode);
			Assert.AreEqual("Group not found.", result.Value);
		}

		[TestMethod]
		public void TestGetGroup_ReturnsInternalServerError_WhenUnexpectedErrorIsThrown()
		{
			var groupId = Guid.NewGuid();
			_mockGroupService.Setup(service => service.GetGroup(groupId)).Throws(new Exception("Internal server error"));

			var result = _controller.GetGroup(groupId) as ObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
			Assert.AreEqual("Internal server error", result.Value);
		}

		[TestMethod]
		public async Task TestCreateGroup_ReturnsOk_WhenGroupIsCreated()
		{
			var group = new GroupCreationDto
			{
				Name = "TestGroup",
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			var groupId = Guid.NewGuid();

			_mockGroupService.Setup(service => service.CreateGroup(group)).ReturnsAsync(groupId);
			_mockGroupService.Setup(service => service.GetGroup(groupId)).Returns(new GroupExistingDto
			{
				Name = group.Name,
				Latitude = group.Latitude,
				Longitude = group.Longitude,
				LocationRange = group.LocationRange,
				Filters = new List<Filter>(),
				JoinCode = 12345678
			});
			var result = await _controller.CreateGroup(group) as OkObjectResult;

			_mockCookies.Verify(c => c.Append("MemberId", It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Once);

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			Assert.AreEqual(groupId, result.Value);
		}

		[TestMethod]
		public async Task TestCreateGroup_ReturnsNotFound_WhenNoRestaurantsFound()
		{
			var group = new GroupCreationDto
			{
				Name = "TestGroup",
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			var groupId = Guid.NewGuid();

			_mockGroupService.Setup(service => service.CreateGroup(group)).Throws(new NoRestaurantsFoundException());
			var result = await _controller.CreateGroup(group) as ObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(404, result.StatusCode);
			Assert.AreEqual(NoRestaurantsFoundException.CustomMessage, result.Value);
		}

		[TestMethod]
		public async Task TestCreateGroup_ReturnsNotFound_WhenGroupNotFound()
		{
			var group = new GroupCreationDto
			{
				Name = "TestGroup",
				Latitude = 47.36667,
				Longitude = 8.55,
				LocationRange = 1000
			};

			var groupId = Guid.NewGuid();

			_mockGroupService.Setup(service => service.CreateGroup(group)).Throws(new GroupNotFoundException());
			var result = await _controller.CreateGroup(group) as ObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(404, result.StatusCode);
			Assert.AreEqual(GroupNotFoundException.CustomMessage, result.Value);
		}
	}
}