using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace StarterKit.Controllers
{
    [ApiController]
	[EnableCors]
	[Produces("application/json")]
	[Route("api/v1/authorize")]
	public class AuthorizeController : ControllerBase
	{
		private readonly IUserContext _userContext;

		public AuthorizeController(IUserContext userContext)
		{
			_userContext = userContext;
		}

		[Authorize]
		[HttpGet("me")]
		[ProducesResponseType(302)]
		[ProducesResponseType(200)]
		public IActionResult Get()
		{
			var userId = _userContext.Username;
			Response.Headers.Add("X-User", userId);
			return Ok();
		}

		[AllowAnonymous]
		[HttpGet("authenticated")]
		[ProducesResponseType(401)]
		[ProducesResponseType(typeof(string), 200)]
		public IActionResult Authenticated()
		{
			if (_userContext.Username != null)
				return Ok(_userContext.Username);
			return StatusCode(401);
		}

		[Authorize]
		[HttpGet("authenticate")]
		[ProducesResponseType(302)]
		[ProducesResponseType(400)]
		public IActionResult GetAuthStatus([FromQuery] string redirectUrl)
		{
			if (redirectUrl == null)
				return BadRequest();
			return Redirect(redirectUrl);
		}
	}
}