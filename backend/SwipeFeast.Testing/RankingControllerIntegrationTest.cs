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
using System.Threading.Tasks;

namespace SwipeFeast.Testing
{
	[TestClass]
	public class RankingControllerIntegrationTest
	{

		[TestMethod]
		public void TestGetListOfRankings_ReturnsOk_WhenRankingsCouldBeReturned()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<RankingController>> mockLogger = new Mock<ILogger<RankingController>>();
            RankingController rankingController = new RankingController(mockGroupService.Object, mockLogger.Object);

			var groupId = Guid.NewGuid();
			var exampleRanking = new Ranking
			{
				RestaurantId = "ChIJN1t_tDeuEmsRUsoyG83frY4",
				RestaurantName = "TestRestaurant",
				LikeCount = 1,
				Members = []
			};
			var expectedRankings = new List<Ranking> { exampleRanking };

			mockGroupService.Setup(service => service.GetListOfRankings(groupId)).Returns(expectedRankings);

			var result = rankingController.GetListOfRankings(groupId);

			Assert.IsTrue(result is OkObjectResult);
		}

		[TestMethod]
		public void TestGetListOfRankings_ReturnsNotFound_WhenGroupNotFound()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<RankingController>> mockLogger = new Mock<ILogger<RankingController>>();
            RankingController rankingController = new RankingController(mockGroupService.Object, mockLogger.Object);

			var groupId = Guid.NewGuid();

			mockGroupService.Setup(service => service.GetListOfRankings(groupId)).Throws(new GroupNotFoundException());

			var result = rankingController.GetListOfRankings(groupId) as NotFoundObjectResult;

			Assert.IsTrue(result is NotFoundObjectResult);
			Assert.AreEqual(404, result.StatusCode);
			Assert.AreEqual(GroupNotFoundException.CustomMessage, result.Value);
		}

		[TestMethod]
		public void TestGetListOfRankings_ReturnsInternalServerError_WhenUnexpectedErrorIsThrown()
		{
			Mock<IGroupService> mockGroupService = new Mock<IGroupService>();
            Mock<ILogger<RankingController>> mockLogger = new Mock<ILogger<RankingController>>();
            RankingController rankingController = new RankingController(mockGroupService.Object, mockLogger.Object);

			var groupId = Guid.NewGuid();

			mockGroupService.Setup(service => service.GetListOfRankings(groupId)).Throws(new Exception());

			var result = rankingController.GetListOfRankings(groupId) as ObjectResult;

			Assert.IsTrue(result is ObjectResult);
			Assert.AreEqual(500, result.StatusCode);
			Assert.AreEqual("Internal server error", result.Value);
		}
	}
}
