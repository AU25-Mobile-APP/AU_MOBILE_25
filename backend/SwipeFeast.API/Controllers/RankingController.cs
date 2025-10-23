using Microsoft.AspNetCore.Mvc;
using SwipeFeast.API.Services;
using SwipeFeast.API.Services.Exceptions;
using System;
using Microsoft.Extensions.Logging;

namespace SwipeFeast.API.Controllers
{
    /// <summary>
    /// Controller for managing the rankings of a group.
    /// </summary>
    public class RankingController : BaseController
    {
        public RankingController(IGroupService groupService, ILogger<RankingController> logger) : base(groupService, logger)
        {
        }

        /// <summary>
        /// Get a List of Restaurants ranked by the Group Members in descending order.
        /// </summary>
        /// <param name="groupId">The unique identifier for the group.</param>
        /// <returns>A list of restaurant rankings.</returns>
        [Route("groups/{groupId}/ranking")]
        [HttpGet]
        public IActionResult GetListOfRankings([FromRoute] Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                _logger.LogError("Invalid group ID provided in request.");
                return BadRequest("Group ID must be set correctly.");
            }

            try
            {
                var restaurantRankings = _groupService.GetListOfRankings(groupId);
                _logger.LogInformation("Successfully fetched rankings for group ID: {GroupId}", groupId);
                return Ok(restaurantRankings);
            }
            catch (GroupNotFoundException exception)
            {
                _logger.LogWarning("Group not found: {GroupId}. Error: {Message}", groupId, exception.Message);
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch rankings for group ID: {GroupId}", groupId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
