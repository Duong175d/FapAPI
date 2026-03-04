using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FAP_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		[Authorize]
		[HttpGet("CheckToken")]
		public IActionResult CheckToken()
		{
			var identity = HttpContext.User.Identity as ClaimsIdentity;
			if (identity == null || !identity.IsAuthenticated)
			{
				return Unauthorized("Invalid Token");
			}

			var claims = identity.Claims.ToDictionary(c => c.Type, c => c.Value);

			return Ok(new
			{
				message = "Token is valid",
				claims
			});
		}
	}
}