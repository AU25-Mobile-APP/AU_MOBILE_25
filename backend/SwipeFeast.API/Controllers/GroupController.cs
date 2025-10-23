using Microsoft.AspNetCore.Mvc;
using SwipeFeast.API.Services;
using SwipeFeast.API.Models;
using System.ComponentModel.DataAnnotations;
using SwipeFeast.API.Services.Exceptions;
using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace SwipeFeast.API.Controllers
{
    /// <summary>
    /// Controller for managing the groups.
    /// </summary>
    [ApiController]
    public class GroupController : BaseController
    {
        /// <summary>
        /// Constructor for the GroupController.
        /// </summary>
        /// <param name="groupService"></param>
        public GroupController(IGroupService groupService, ILogger<GroupController> logger) : base(groupService, logger) { }

        /// <summary>
        /// Creates a new group based on location and a set of filters.
        /// </summary>
        /// <param name="group">Group data including location and filters</param>
        /// <returns>Returns a group-ID as confirmation of group creation.</returns>
        [Route("groups")]
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupCreationDto group)
        {
            try
            {
                if (group == null)
                {
                    _logger.LogError("Failed to create group: group data is null.");
                    return BadRequest("Group is required");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid while creating group.");
                    return BadRequest(ModelState);
                }

                var groupId = await _groupService.CreateGroup(group);

                var memberId = Guid.NewGuid();
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddMinutes(300)
                };
                _groupService.AddMemberToGroup(memberId, _groupService.GetGroup(groupId)!.JoinCode);

                Response.Cookies.Append("MemberId", memberId.ToString(), cookieOptions);
                _logger.LogInformation("New group created with ID: {GroupId}", groupId);
                return Ok(groupId);
            }
            catch (NoRestaurantsFoundException exception)
            {
                _logger.LogWarning("No restaurants found: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
            catch (GroupNotFoundException exception)
            {
                _logger.LogWarning("Group not found: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
            catch (FilterInvalidException exception)
            {
                _logger.LogError("Invalid filters submitted: {Message}", exception.Message);
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to create group due to an unexpected error.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all group meta data.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>Special group object containing meta data.</returns>
        [Route("groups/{groupId}")]
        [HttpGet]
        public IActionResult GetGroup([FromRoute] Guid groupId)
        {
            try
            {
                var group = _groupService.GetGroup(groupId);
                if (group == null)
                {
                    _logger.LogWarning("Group not found with ID: {GroupId}", groupId);
                    return NotFound("Group not found");
                }
                _logger.LogInformation("Group data retrieved for ID: {GroupId}", groupId);
                return Ok(group);
            }
            catch (GroupNotFoundException exception)
            {
                _logger.LogWarning("Group not found: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to retrieve group data.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Member joins the group
        /// </summary>
        /// <param name="joinCode"></param>
        /// <returns>No Content</returns>
        [Route("groups/{joinCode}/join")]
        [HttpPost]
        public IActionResult JoinMember([FromRoute] int joinCode)
        {
            if (Request.Cookies["MemberId"] != null)
            {
                _logger.LogWarning("Attempt to join a group when already in another group.");
                return BadRequest("Member is already in a group");
            }

            var memberId = Guid.NewGuid();
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(300)
            };
            Response.Cookies.Append("MemberId", memberId.ToString(), cookieOptions);

            try
            {
                _logger.LogInformation("Member {MemberId} joined the group with join code {JoinCode}", memberId, joinCode);
                return Ok(_groupService.AddMemberToGroup(memberId, joinCode));
            }
            catch (GroupNotFoundException exception)
            {
                _logger.LogWarning("Failed to join group: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to join group.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Member leaves the group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>No Content</returns>
        [Route("groups/{groupId}/leave")]
        [HttpPost]
        public IActionResult LeaveMember([FromRoute] Guid groupId)
        {
            var memberId = Request.Cookies["MemberId"];
            if (string.IsNullOrEmpty(memberId))
            {
                _logger.LogWarning("Attempt to leave group without member ID.");
                return Unauthorized("Member ID is required to leave the group.");
            }

            try
            {
                _groupService.RemoveMemberFromGroup(groupId, new Guid(memberId));
                Response.Cookies.Delete("MemberId");
                _logger.LogInformation("Member {MemberId} left the group {GroupId}", memberId, groupId);
                return NoContent();
            }
            catch (MemberNotFoundException exception)
            {
                _logger.LogWarning("Member not found: {Message}", exception.Message);
                return Unauthorized(exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to leave group.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
