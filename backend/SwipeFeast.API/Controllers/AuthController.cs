using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SwipeFeast.API.Services;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services.Exceptions;

namespace SwipeFeast.API.Controllers
{
    /// <summary>
    /// Controller for managing Authentication
    /// </summary>
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Constructor for the AuthController.
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="groupService"></param>
        /// <param name="baseLogger"></param>
        /// <param name="authLogger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AuthController(
            IAuthService authService,
            IGroupService groupService,
            ILogger<BaseController> baseLogger,
            ILogger<AuthController> authLogger)
            : base(groupService, baseLogger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = authLogger ?? throw new ArgumentNullException(nameof(authLogger));
        }

        /// <summary>
        /// Register a user in the firstoreDB if not already registered.
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        [HttpPost("auth")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto,
            CancellationToken cancellationToken)
        {
            try
            {
                if (registerDto == null)
                {
                    _logger.LogError("Failed to register new user: registration data is null");
                    return BadRequest("Registration data is required");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid while registering new user");
                    return BadRequest(ModelState);
                }

                await _authService.CreateUserIfNotExistsAsync(registerDto, cancellationToken);
                return Ok();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Registration request was canceled.");
                // 499 Client Closed Request (a good fit, but not built-in)
                return StatusCode(499, "Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register new user");
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("auth")]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                // Versuche mehrere mögliche Claim-Namen (Firebase liefert oft "user_id" oder "sub")
                var userUid = User.FindFirst("user_id")?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("uid")?.Value
                              ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrWhiteSpace(userUid))
                {
                    _logger.LogWarning("User UID not found in token");
                    return Unauthorized("User id not found in token");
                }

                // Anpassung: passe den Servicenamen an deine Implementierung an
                var user = await _authService.GetUserByUidAsync(userUid);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user data");
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut(("auth"))]
        public async Task<IActionResult> GetUserData([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Versuche mehrere mögliche Claim-Namen (Firebase liefert oft "user_id" oder "sub")
                var userUid = User.FindFirst("user_id")?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("uid")?.Value
                              ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrWhiteSpace(userUid))
                {
                    _logger.LogWarning("User UID not found in token");
                    return Unauthorized("User id not found in token");
                }

                if (userUid != registerDto.UserUID)
                    throw new InvalidDtoException("userId in request doesnt match account userId");

                // Anpassung: passe den Servicenamen an deine Implementierung an
                var user = await _authService.PatchUser(registerDto);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user data");
                return BadRequest(ex.Message);
            }
        }
// csharp
        [HttpGet("groupid")]
        public async Task<IActionResult> GetGroupId()
        {
            try
            {
                var userUid = User.FindFirst("user_id")?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("uid")?.Value
                              ?? User.FindFirst("sub")?.Value;
        
                if (string.IsNullOrWhiteSpace(userUid))
                {
                    _logger.LogWarning("User UID not found in token when requesting group id");
                    return Unauthorized("User id not found in token");
                }
        
                var groupId = await _authService.GetGroupIdByUIDAsync(userUid);
                if (groupId == null)
                    return NotFound();
        
                return Ok(groupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get group id");
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("groupid")]
        public async Task<IActionResult> PatchGroupId([FromBody] GroupIDDto groupDto)
        {
            try
            {
                if (groupDto == null)
                {
                    _logger.LogWarning("PatchGroupId called with null body");
                    return BadRequest("Request body is required");
                }
        
                var userUid = User.FindFirst("user_id")?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("uid")?.Value
                              ?? User.FindFirst("sub")?.Value;
        
                if (string.IsNullOrWhiteSpace(userUid))
                {
                    _logger.LogWarning("User UID not found in token when patching group id");
                    return Unauthorized("User id not found in token");
                }
        
                if (groupDto.UserUID != userUid)
                    throw new InvalidDtoException("userId in request doesn't match token userId");
        
                var patched = await _authService.PatchGroupIdByUIDAsync(groupDto);
                if (patched == null)
                    return NotFound();
        
                return Ok(patched);
            }
            catch (InvalidDtoException ex)
            {
                _logger.LogWarning(ex, "Invalid DTO while patching group id");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to patch group id");
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("groupid")]
        public async Task<IActionResult> CreateGroupIdIfNotExists([FromBody] GroupIDDto groupDto, CancellationToken cancellationToken)
        {
            try
            {
                if (groupDto == null)
                {
                    _logger.LogWarning("CreateGroupIdIfNotExists called with null body");
                    return BadRequest("Request body is required");
                }
        
                var userUid = User.FindFirst("user_id")?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("uid")?.Value
                              ?? User.FindFirst("sub")?.Value;
        
                if (string.IsNullOrWhiteSpace(userUid))
                {
                    _logger.LogWarning("User UID not found in token when creating group id");
                    return Unauthorized("User id not found in token");
                }
        
                // Setze die UserUID serverseitig, falls der Client sie nicht lieftert
                groupDto.UserUID = userUid;
        
                await _authService.CreateGroupIdIfNotExistsAsync(groupDto, cancellationToken);
                return Ok();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("CreateGroupIdIfNotExists request was canceled.");
                return StatusCode(499, "Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create group id");
                return BadRequest(ex.Message);
            }
        }
        
        
    }
}