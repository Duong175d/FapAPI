using FAP_API.Constants;
using FAP_API.Models;
using FAP_API.Services;
using FAP_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FAP_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController(IUserService _userService) : ControllerBase
    {


        [HttpGet] 
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var username = _userService.GetUserName(userId).UserName;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("Không có username trong token.");

            var user = _userService.GetUserByUserName(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            return Ok(user);

        }

        [HttpGet] 
        [Authorize(Roles = RoleString.Admin)]
        public ActionResult<List<AccountViewModel>> GetUserList()
        {
            var users = _userService.GetUserList();
            return Ok(users);
        }

        [HttpGet]
        [Authorize(Roles = RoleString.Admin)]
        public IActionResult GetAccountDetail([FromQuery] string username)
        {
            var account = _userService.GetUserDetailByUserName(username);
            if (account == null)
                return NotFound("Không tìm thấy người dùng");

            return Ok(account);
        }

        [HttpPut]
        [Authorize(Roles = RoleString.Admin)]
        public IActionResult UpdateUser([FromBody] AccountDetail model)
        {
            var result = _userService.UpdateUser(model);
            if (!result)
                return NotFound("Không tìm thấy người dùng để cập nhật");

            return Ok("Cập nhật thành công");
        }

        [HttpGet]
        [Authorize(Roles = RoleString.Admin)]
        public IActionResult GetRoleList()
        {
            return Ok(_userService.GetRoleList());
        }

        [HttpGet]
        [Authorize(Roles = RoleString.Admin)]
        public IActionResult GetStudentStatusList()
        {
            return Ok(_userService.GetStudentStatusList());
        }

        [HttpGet]
        [Authorize(Roles = RoleString.Admin)]
        public IActionResult GetTeacherStatusList()
        {
            return Ok(_userService.GetTeacherStatusList());
        }

    }
}
