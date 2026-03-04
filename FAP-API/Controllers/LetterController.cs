using FAP_API.Models;
using FAP_API.Services;
using FAP_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FAP_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class LetterController : ControllerBase
	{
		private readonly ILetterService _letterService;

		public LetterController(ILetterService letterService)
		{
			_letterService = letterService;
		}

		//GET
		[HttpGet]
		public ActionResult<IEnumerable<Letter>> GetLetters()
		{
			var letters = _letterService.GetLetterList();
			return Ok(letters);
		}

		[HttpGet]
		public ActionResult<IEnumerable<Letter>> GetLettersByUser()
		{
			// Lấy UserId từ Claim
			var userId = User.FindFirst("UserId")?.Value;
			if (userId == null) return Unauthorized("User ID not found in token");

			var letters = _letterService.GetLettersByUserId(userId);
			return Ok(letters);
		}

		[HttpGet]
		public ActionResult<IEnumerable<Letter>> GetLettersByStatus(string status)
		{
			var letters = _letterService.GetLettersByStatus(status);
			return Ok(letters);
		}

		[HttpGet]
		public ActionResult<IEnumerable<Letter>> GetLettersByType(string type)
		{
			var letters = _letterService.GetLettersByType(type);
			return Ok(letters);
		}

		[HttpGet]
		public IActionResult GetTypeLetterDescription()
		{
			var letters = _letterService.GetTypeLetterDescriptions();
			return Ok(letters);
		}

		[HttpGet]
		public IActionResult GetInfoByUserId()
		{
			var userId = User.FindFirst("UserId")?.Value;
			if (userId == null)
				return Unauthorized("User ID not found in token.");

			var userInfo = _letterService.GetInfoByUserId(userId);
			return Ok(userInfo);
		}

		[HttpGet]
		public IActionResult GetLettersByLetterId(string letterId)
		{
			var letter = _letterService.GetLetterByLetterId(letterId);
			return Ok(letter);
		}

		//PUT
		[HttpPut]
		public IActionResult UpdateStatus([FromQuery] string letterId, [FromQuery] string newStatus)
		{
			var result = _letterService.UpdateLetterStatus(letterId, newStatus);
			if (!result)
				return NotFound("Letter not found");

			return Ok("Status updated successfully");
		}

		[HttpPut]
		public IActionResult UpdateResponse([FromQuery] string letterId, [FromQuery] string response)
		{
			var result = _letterService.UpdateLetterResponse(letterId, response);
			if (!result)
				return NotFound("Letter not found");

			return Ok("Response updated successfully");
		}

		//POST
		[HttpPost]
		public IActionResult CreateLetter([FromBody] LetterViewModel viewModel)
		{
			var userId = User.FindFirst("UserId")?.Value;
			if (userId == null)
				return Unauthorized("User ID not found in token.");

			_letterService.CreateLetter(userId, viewModel);
			return Ok("Letter created successfully.");
		}
	}
}
