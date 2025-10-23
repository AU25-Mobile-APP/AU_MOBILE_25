using Microsoft.AspNetCore.Mvc;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services;
using SwipeFeast.API.Services.Exceptions;
using System;
using System.Text.Json;

namespace SwipeFeast.API.Controllers
{
    /// <summary>
    /// Controller for managing the restaurants of a group.
    /// </summary>
    /// <remarks>
    /// Constructor for the RestaurantController.
    /// </remarks>
    /// <param name="groupService"></param>
    [ApiController]
	public class RestaurantController(IGroupService groupService, ILogger<RestaurantController> logger) : BaseController(groupService, logger)
	{

        /// <summary>
        /// Receive a list of restaurants and assign them to an active group.
        /// </summary>
        /// <param name="groupId">ID of group as a generated ID.</param>
        /// <returns>List of restaurants</returns>
        [Route("groups/{groupId}/restaurants")]
		[HttpGet]
		public ActionResult<List<Restaurant>> GetRestaurants([FromRoute] Guid groupId)
		{
			var restaurants = new List<Restaurant>();
			var memberId = Request.Cookies["MemberId"];
            if (string.IsNullOrEmpty(memberId))
            {
                _logger.LogWarning("Unauthorized attempt to access restaurants without member ID.");
                return Unauthorized();
            }

            try
            {
                restaurants = _groupService.GetRestaurants(groupId, Guid.Parse(memberId));
                _logger.LogInformation("Successfully fetched restaurants for group ID: {GroupId}", groupId);
                return Ok(restaurants);
            }
            catch (GroupNotFoundException exception)
            {
                _logger.LogWarning("Group not found: {GroupId}. Error: {Message}", groupId, exception.Message);
                return NotFound(exception.Message);
            }
            catch (MemberNotFoundException exception)
            {
                _logger.LogWarning("Member not found: {MemberId}. Error: {Message}", memberId, exception.Message);
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch restaurants for group ID: {GroupId}", groupId);
                return BadRequest($"The request data did not meet expectations regarding necessary properties: {exception.Message}");
            }
        }

        /// <summary>
        /// Get a specific restaurant by ID.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        [Route("groups/{groupId}/restaurants/{restaurantId}")]
        [HttpGet]
        public ActionResult<Restaurant> GetRestaurant([FromRoute] Guid groupId, [FromRoute] string restaurantId)
        {
            if (groupId == Guid.Empty)
            {
                _logger.LogError("Invalid group ID provided.");
                return BadRequest("Group ID must be set correctly.");
            }
            if (string.IsNullOrEmpty(restaurantId))
            {
                _logger.LogError("No restaurant ID provided.");
                return BadRequest("Restaurant ID is required.");
            }

            var memberId = Request.Cookies["MemberId"];
            if (string.IsNullOrEmpty(memberId))
            {
                _logger.LogWarning("Unauthorized attempt to access restaurant without member ID.");
                return Unauthorized();
            }

            try
            {
                var restaurant = _groupService.GetRestaurant(groupId, Guid.Parse(memberId), restaurantId);
                _logger.LogInformation("Successfully fetched restaurant {RestaurantId} for group ID: {GroupId}", restaurantId, groupId);
                return Ok(restaurant);
            }
            catch (GroupNotFoundException exception)
            {
                _logger.LogWarning("Group not found: {GroupId}. Error: {Message}", groupId, exception.Message);
                return NotFound(exception.Message);
            }
            catch (MemberNotFoundException exception)
            {
                _logger.LogWarning("Member not found: {MemberId}. Error: {Message}", memberId, exception.Message);
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch restaurant {RestaurantId} for group ID: {GroupId}", restaurantId, groupId);
                return BadRequest($"The request data did not meet expectations regarding necessary properties: {exception.Message}");
            }
        }

        /// <summary>
        /// Increase ranking of restaurant for a specific active group by one.
        /// </summary>
        /// <param name="groupId">ID of group as a generated ID.</param>
        /// <param name="restaurantId">ID of restaurant.</param>
        /// <returns></returns>
        [Route("groups/{groupId}/restaurants/{restaurantId}/like")]
        [HttpPost]
        public ActionResult LikeRestaurant([FromRoute] Guid groupId, [FromRoute] string restaurantId)
        {
            if (groupId == Guid.Empty)
            {
                _logger.LogError("Invalid group ID provided.");
                return BadRequest("Group ID must be set correctly.");
            }
            if (string.IsNullOrEmpty(restaurantId))
            {
                _logger.LogError("No restaurant ID provided.");
                return BadRequest("Restaurant ID is required.");
            }

            var memberId = Request.Cookies["MemberId"];
            if (string.IsNullOrEmpty(memberId))
            {
                _logger.LogWarning("Unauthorized attempt to like a restaurant without member ID.");
                return Unauthorized();
            }

            try
            {
                _groupService.IncreaseLikeForRestaurant(groupId, Guid.Parse(memberId), restaurantId);
                _logger.LogInformation("Increased like for restaurant {RestaurantId} by member ID: {MemberId}", restaurantId, memberId);
                return Ok();
            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(GroupNotFoundException) || exception.GetType() == typeof(RestaurantNotFoundException))
                {
                    _logger.LogWarning("Not found exception during liking: {Message}", exception.Message);
                    return NotFound(exception.Message);
                }
                if (exception.GetType() == typeof(MemberNotFoundException))
                {
                    _logger.LogWarning("Unauthorized member during liking: {Message}", exception.Message);
                    return Unauthorized(exception.Message);
                }
                _logger.LogError(exception, "Failed to like restaurant {RestaurantId} by member ID: {MemberId}", restaurantId, memberId);
                return BadRequest($"The request data did not meet expectations regarding necessary properties: {exception.Message}");
            }
        }
    }
}