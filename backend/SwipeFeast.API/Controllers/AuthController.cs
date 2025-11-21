using Microsoft.AspNetCore.Mvc;
using SwipeFeast.API.Services;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services.Exceptions;

namespace SwipeFeast.API.Controllers
{
    /// <summary>
    /// Controller for managing Authentication
    /// </summary>
    [ApiController]
    public class AuthController: BaseController
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

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto registration)
        {
            try
            {
                if (registration == null)
                {
                    _logger.LogError("Failed to register new user: registration data is null");
                    return BadRequest("Registration data is required");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid while registering new user");
                    return BadRequest(ModelState);
                }

                var user = await _authService.Register(registration);
                return Ok(user.GetValue<string>("FirstName"));



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register new user");
                return BadRequest(ex.Message);
            }
            
        }


    }
}

