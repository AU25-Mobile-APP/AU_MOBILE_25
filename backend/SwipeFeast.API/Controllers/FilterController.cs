using Microsoft.AspNetCore.Mvc;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services;
using SwipeFeast.API.Services.Exceptions;
using System.Net;
using System;

namespace SwipeFeast.API.Controllers
{
    /// <summary>
	/// Controller for managing the filters of a group.
	/// </summary>
	/// <param name="groupService"></param>
    [ApiController]
    public class FilterController(IGroupService groupService, ILogger<FilterController> logger) : BaseController(groupService, logger)
    {
   

        /// <summary>
        /// Receive a list of available Food Types filters.
        /// </summary>
        /// <returns></returns>
        [Route("filters/{groupId}")]
        [HttpGet]
        public IActionResult Get([FromRoute] Guid groupId)
        {
            try
            {
                var filters = _groupService.GetFilters(groupId);
                _logger.LogInformation("Successfully fetched filters for group ID: {GroupId}", groupId);
                return Ok(filters);
            }
            catch (GroupNotFoundException exception)
            {
                _logger.LogWarning("Group not found: {GroupId}. Error: {Message}", groupId, exception.Message);
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch filters for group ID: {GroupId}", groupId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update the list of available Food Types filters.
        /// </summary>
        /// <param name="groupId">Group ID</param>
        /// <param name="filters">List of filters</param>
        /// <returns></returns>
        [Route("filters/{groupId}")]
        [HttpPatch]
        public async Task<ActionResult> Patch([FromRoute] Guid groupId, [FromBody] List<Filter> filters)
        {
            var memberId = Request.Cookies["MemberId"];
            if (memberId == null)
            {
                _logger.LogWarning("Unauthorized attempt to modify filters without member ID.");
                return Unauthorized();
            }

            try
            {
                await _groupService.SetFilters(groupId, new Guid(memberId), filters);
                _logger.LogInformation("Updated filters for group ID: {GroupId} by member ID: {MemberId}", groupId, memberId);
                return NoContent();
            }
            catch (GroupNotFoundException exception)
            {
                _logger.LogWarning("Group not found during update: {GroupId}. Error: {Message}", groupId, exception.Message);
                return NotFound(exception.Message);
            }
            catch (NoRestaurantsFoundException exception)
            {
                _logger.LogWarning("No restaurants found matching filters for group ID: {GroupId}. Error: {Message}", groupId, exception.Message);
                return NotFound(exception.Message);
            }
            catch (FilterInvalidException exception)
            {
                _logger.LogError("Invalid filters submitted for group ID: {GroupId}. Error: {Message}", groupId, exception.Message);
                return BadRequest(exception.Message);
            }
            catch (MemberNotFoundException)
            {
                _logger.LogWarning("Member not found with ID: {MemberId} during filter update.", memberId);
                return Unauthorized();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while updating filters for group ID: {GroupId} by member ID: {MemberId}", groupId, memberId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get the default filters for create a group.
        /// </summary>
        /// <returns>give a List of default filters</returns>
        [Route("defaultfilters")]
        [HttpGet]
        public IActionResult GetDefaultFilters()
        {
            try
            {
                var filters = _groupService.GetDefaultFilters();
                _logger.LogInformation("Fetching default filters");
                return Ok(filters);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch default filters");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
