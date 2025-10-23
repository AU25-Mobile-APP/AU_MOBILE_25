using Microsoft.AspNetCore.Mvc;
using SwipeFeast.API.Services;

namespace SwipeFeast.API.Controllers
{
	// TODO: Add Swagger documentation
	/// <summary>
	/// Base controller for all controllers in the API.
	/// </summary>
	[Route("api/v1/")]
	[ApiController]
	public class BaseController : Controller
	{
        public readonly ILogger _logger;
        protected IGroupService _groupService;

		/// <summary>
		///	Constructor for the base controller.
		/// </summary>
		/// <param name="groupService"></param>
		public BaseController(IGroupService groupService, ILogger logger)
		{
			_groupService = groupService;
			_logger = logger;
		}
	}
}
