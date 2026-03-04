using FAP_API.Constants;
using FAP_API.ModelDTO;
using FAP_API.Models;
using FAP_API.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;

namespace FAP_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenController(JwtToken _jwtToken, Prm392Context _prm392Context) : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginInfo loginInfor)
        {
            try
            {
                var token = _jwtToken.GenarateToken(loginInfor);
                var user = _prm392Context.Users.FirstOrDefault(u => u.Email == loginInfor.Email);
                return Ok(new
                {
                    token = token,
                    user = new
                    {
                        user.UserId,
                        user.Email,
                        user.FullName,
                        user.UserName,
                        Role = user.Role,
                    }   
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        } 
    }
}
