using FAP_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FAP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult GetListNotification(string? search)
        {
            try
            {             
                var ltNotification = _notificationService.GetList(search);
                return Ok(ltNotification);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("DetailNotification")]
        public IActionResult GetDetailNotification(string? notificationId)
        {
            try
            {
                var ltNotification = _notificationService.GetDetaiById(notificationId);
                return Ok(ltNotification);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
