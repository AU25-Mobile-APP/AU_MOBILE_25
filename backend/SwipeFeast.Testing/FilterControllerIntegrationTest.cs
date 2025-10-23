using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SwipeFeast.API.Controllers;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services;
using SwipeFeast.API.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwipeFeast.Testing
{
	[TestClass]
	public class FilterControllerIntegrationTest
	{
		[TestMethod]
		public void TestGetFilters_Successful()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object);

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

			mockGroupService.Setup(service => service.GetFilters(groupId)).Returns(new List<Filter> { exampleDefaultFilter });

			var result = filterController.Get(groupId);

			Assert.IsTrue(result is OkObjectResult);
		}

		[TestMethod]
		public void TestGetFilters_Unsuccessful_GroupNotFound()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object);

			var groupId = Guid.NewGuid();

			mockGroupService.Setup(service => service.GetFilters(groupId)).Throws(new GroupNotFoundException());

			var result = filterController.Get(groupId) as NotFoundObjectResult;

			Assert.IsTrue(result is NotFoundObjectResult);
			Assert.AreEqual(404, result.StatusCode);
			Assert.AreEqual(GroupNotFoundException.CustomMessage, result.Value);
		}

		[TestMethod]
		public void TestGetFilters_Unsuccessful_InternalServerError()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object);

			var groupId = Guid.NewGuid();

			mockGroupService.Setup(service => service.GetFilters(groupId)).Throws(new Exception());

			var result = filterController.Get(groupId) as ObjectResult;

			Assert.IsTrue(result is ObjectResult);
			Assert.AreEqual(500, result.StatusCode);
			Assert.AreEqual("Internal server error", result.Value);
		}

		[TestMethod]
		public async Task TestPatch_ReturnsNoContent_WhenCouldPatchFilters()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            Mock<IResponseCookies> mockCookies;
			Mock<HttpContext> mockHttpContext;
			Mock<HttpResponse> mockHttpResponse;

			mockGroupService = new Mock<IGroupService>();
			mockCookies = new Mock<IResponseCookies>();
			mockHttpContext = new Mock<HttpContext>();
			mockHttpResponse = new Mock<HttpResponse>();
			mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockCookies.Object);

			// Mock request cookies
			var cookieCollection = new Mock<IRequestCookieCollection>();
			cookieCollection.Setup(c => c["MemberId"]).Returns(Guid.NewGuid().ToString());
			mockHttpContext.SetupGet(x => x.Request.Cookies).Returns(cookieCollection.Object);

			FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = mockHttpContext.Object
				}
			};

			var filters = new List<Filter> { new Filter { Id = "Filter1", Active = true } };
			var groupId = Guid.NewGuid();

			mockGroupService.Setup(service => service.SetFilters(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<Filter>>()));

			var result = await filterController.Patch(groupId, filters) as NoContentResult;

			Assert.IsNotNull(result);
			Assert.IsTrue(result is NoContentResult);
			Assert.AreEqual(204, result.StatusCode);
		}


		[TestMethod]
		public async Task TestPatch_ReturnsNotFound_WhenGroupNotFound()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            Mock<IResponseCookies> mockCookies;
			Mock<HttpContext> mockHttpContext;
			Mock<HttpResponse> mockHttpResponse;

			mockGroupService = new Mock<IGroupService>();
			mockCookies = new Mock<IResponseCookies>();
			mockHttpContext = new Mock<HttpContext>();
			mockHttpResponse = new Mock<HttpResponse>();
			mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockCookies.Object);

			// Mock request cookies
			var cookieCollection = new Mock<IRequestCookieCollection>();
			cookieCollection.Setup(c => c["MemberId"]).Returns(Guid.NewGuid().ToString());
			mockHttpContext.SetupGet(x => x.Request.Cookies).Returns(cookieCollection.Object);

			FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = mockHttpContext.Object
				}
			};

			mockGroupService.Setup(service => service.SetFilters(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<Filter>>())).Throws(new GroupNotFoundException());

			var result = await filterController.Patch(Guid.NewGuid(), new List<Filter>()) as NotFoundObjectResult;

			Assert.IsTrue(result is NotFoundObjectResult);
			Assert.AreEqual(404, result.StatusCode);
			Assert.AreEqual(GroupNotFoundException.CustomMessage, result.Value);
		}

		[TestMethod]
		public async Task TestPatch_ReturnsNotFound_WhenNoRestaurantsFound()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            Mock<IResponseCookies> mockCookies;
			Mock<HttpContext> mockHttpContext;
			Mock<HttpResponse> mockHttpResponse;

			mockGroupService = new Mock<IGroupService>();
			mockCookies = new Mock<IResponseCookies>();
			mockHttpContext = new Mock<HttpContext>();
			mockHttpResponse = new Mock<HttpResponse>();
			mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockCookies.Object);

			// Mock request cookies
			var cookieCollection = new Mock<IRequestCookieCollection>();
			cookieCollection.Setup(c => c["MemberId"]).Returns(Guid.NewGuid().ToString());
			mockHttpContext.SetupGet(x => x.Request.Cookies).Returns(cookieCollection.Object);

			FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = mockHttpContext.Object
				}
			};

			mockGroupService.Setup(service => service.SetFilters(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<Filter>>())).Throws(new NoRestaurantsFoundException());

			var result = await filterController.Patch(Guid.NewGuid(), new List<Filter>()) as NotFoundObjectResult;

			Assert.IsTrue(result is NotFoundObjectResult);
			Assert.AreEqual(404, result.StatusCode);
			Assert.AreEqual(NoRestaurantsFoundException.CustomMessage, result.Value);
		}

		[TestMethod]
		public async Task TestPatch_ReturnsBadRequest_WhenFilterInvalid()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            Mock<IResponseCookies> mockCookies;
			Mock<HttpContext> mockHttpContext;
			Mock<HttpResponse> mockHttpResponse;

			mockGroupService = new Mock<IGroupService>();
			mockCookies = new Mock<IResponseCookies>();
			mockHttpContext = new Mock<HttpContext>();
			mockHttpResponse = new Mock<HttpResponse>();
			mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockCookies.Object);

			// Mock request cookies
			var cookieCollection = new Mock<IRequestCookieCollection>();
			cookieCollection.Setup(c => c["MemberId"]).Returns(Guid.NewGuid().ToString());
			mockHttpContext.SetupGet(x => x.Request.Cookies).Returns(cookieCollection.Object);

			FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = mockHttpContext.Object
				}
			};

			mockGroupService.Setup(service => service.SetFilters(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<Filter>>())).Throws(new FilterInvalidException());

			var result = await filterController.Patch(Guid.NewGuid(), new List<Filter>()) as BadRequestObjectResult;

			Assert.IsTrue(result is BadRequestObjectResult);
			Assert.AreEqual(400, result.StatusCode);
			Assert.AreEqual(FilterInvalidException.CustomMessage, result.Value);
		}

		[TestMethod]
		public async Task TestPatch_ReturnsUnauthorized_WhenMemberNotfound()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            Mock<IResponseCookies> mockCookies;
			Mock<HttpContext> mockHttpContext;
			Mock<HttpResponse> mockHttpResponse;

			mockGroupService = new Mock<IGroupService>();
			mockCookies = new Mock<IResponseCookies>();
			mockHttpContext = new Mock<HttpContext>();
			mockHttpResponse = new Mock<HttpResponse>();
			mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockCookies.Object);

			// Mock request cookies
			var cookieCollection = new Mock<IRequestCookieCollection>();
			cookieCollection.Setup(c => c["MemberId"]).Returns(Guid.NewGuid().ToString());
			mockHttpContext.SetupGet(x => x.Request.Cookies).Returns(cookieCollection.Object);

			FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = mockHttpContext.Object
				}
			};

			mockGroupService.Setup(service => service.SetFilters(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny <List<Filter>>())).Throws(new MemberNotFoundException());

			var result = await filterController.Patch(Guid.NewGuid(), new List<Filter>()) as UnauthorizedResult;

			Assert.IsTrue(result is UnauthorizedResult);
			Assert.AreEqual(401, result.StatusCode);
		}

		[TestMethod]
		public async Task TestPatch_ReturnsInternalServerError_WhenUnexpectedExceptionIsThrown()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            Mock<IResponseCookies> mockCookies;
			Mock<HttpContext> mockHttpContext;
			Mock<HttpResponse> mockHttpResponse;

			mockGroupService = new Mock<IGroupService>();
			mockCookies = new Mock<IResponseCookies>();
			mockHttpContext = new Mock<HttpContext>();
			mockHttpResponse = new Mock<HttpResponse>();
			mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockCookies.Object);

			// Mock request cookies
			var cookieCollection = new Mock<IRequestCookieCollection>();
			cookieCollection.Setup(c => c["MemberId"]).Returns(Guid.NewGuid().ToString());
			mockHttpContext.SetupGet(x => x.Request.Cookies).Returns(cookieCollection.Object);

			FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = mockHttpContext.Object
				}
			};

			mockGroupService.Setup(service => service.SetFilters(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<Filter>>())).Throws(new Exception());

			var result = await filterController.Patch(Guid.NewGuid(), new List<Filter>()) as ObjectResult;

			Assert.IsTrue(result is ObjectResult);
			Assert.AreEqual(500, result.StatusCode);
			Assert.AreEqual("Internal server error", result.Value);
		}

		[TestMethod]
		public async Task TestPatch_ReturnsUnauthorized_WhenCookieIsNotSet()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            Mock<IResponseCookies> mockCookies;
			Mock<HttpContext> mockHttpContext;
			Mock<HttpResponse> mockHttpResponse;

			mockGroupService = new Mock<IGroupService>();
			mockCookies = new Mock<IResponseCookies>();
			mockHttpContext = new Mock<HttpContext>();
			mockHttpResponse = new Mock<HttpResponse>();
			mockHttpResponse.SetupGet(r => r.Cookies).Returns(mockCookies.Object);

			// Mock request cookies without setting the "MemberId" cookie
			var cookieCollection = new Mock<IRequestCookieCollection>();
			mockHttpContext.SetupGet(x => x.Request.Cookies).Returns(cookieCollection.Object);

			FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = mockHttpContext.Object
				}
			};

			var result = await filterController.Patch(Guid.NewGuid(), new List<Filter>()) as UnauthorizedResult;

			Assert.IsTrue(result is UnauthorizedResult);
			Assert.AreEqual(401, result.StatusCode);
		}

		[TestMethod]
		public void TestGetDefaultFilter_ReturnsOk_WhenDefaultFilterIsFound()
		{
			var defaultFilters = GoogleService.GetDefaultFilters();
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<FilterController>> mockLogger = new Mock<ILogger<FilterController>>();
            FilterController filterController = new FilterController(mockGroupService.Object, mockLogger.Object);
			mockGroupService.Setup(mockGroupService => mockGroupService.GetDefaultFilters()).Returns(defaultFilters);

			var result = filterController.GetDefaultFilters() as OkObjectResult;
			Assert.IsNotNull(defaultFilters);
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Value);

			var retrievedFilters = result.Value as List<Filter>;

			for (int i = 0; i < defaultFilters.Count; i++)
			{
				Assert.AreEqual(defaultFilters[i].Id, retrievedFilters[i].Id);
				Assert.AreEqual(defaultFilters[i].Active, retrievedFilters[i].Active);
			}


			Assert.IsTrue(result is OkObjectResult);
			Assert.AreEqual(200, result.StatusCode);
		}
	}
}
