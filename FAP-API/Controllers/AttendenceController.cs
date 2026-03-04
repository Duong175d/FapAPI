using FAP_API.Constants;
using FAP_API.Models;
using FAP_API.Services;
using FAP_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FAP_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendenceController : ControllerBase
    {
        private readonly IAttendenceService _attendenceService;
        public AttendenceController(IAttendenceService attendenceService)
        {
            _attendenceService = attendenceService;
        }
        //[Authorize(Roles = ConstantsDescription.ROLE_TEACHER)]
        [HttpGet("getAttendanceStatus")]
        public IActionResult GetAttendanceStatus()
        {
            var msg = _attendenceService.GetListAttendanceStatus(out List<SystemKey> attendanceStatuses);
            if (msg.Length > 0) return Ok(msg);

            return Ok(attendanceStatuses);
        }
        [Authorize(Roles = ConstantsDescription.ROLE_ADMIN)]
        [HttpPost("ChangeAttendenceForAdmin")]
        public IActionResult ChangeAttendenceAdmin([FromBody] AttendenceViewModel request)
        {
            if (string.IsNullOrEmpty(request.timeTableId) || (request.attendenceStatusCode != 19 
                && request.attendenceStatusCode != 20 && request.attendenceStatusCode != 21))
            {
                return Ok("Invalid input parameters.");
            }

            var msg = _attendenceService.ChangeAttendanceStatus(request.timeTableId, request.attendenceStatusCode);
            if (msg.Length > 0) return Ok(msg);

            return Ok("Đã đổi trạng thái điểm danh");
        }
        //[Authorize(Roles = ConstantsDescription.ROLE_TEACHER)]
        [HttpPost("ChangeAttendenceForTeacher")]
        public IActionResult ChangeAttendenceTeacher([FromBody] AttendenceViewModel request)
        {
            if (string.IsNullOrEmpty(request.timeTableId) || (request.attendenceStatusCode != 19
                && request.attendenceStatusCode != 20 && request.attendenceStatusCode != 21))
            {
                return Ok("Invalid input parameters.");
            }
            var timetable = _attendenceService.GetTimeTableById(request.timeTableId);
            if (timetable == null) return Ok("Không tìm thấy lịch học");

            if (timetable.DateTime.ToDateTime(TimeOnly.MinValue).AddDays(1) < DateTime.Now)
            {
                return Ok("Không thể đổi trạng thái điểm danh cho lịch học đã qua 1 ngày");
            }
            var msg = _attendenceService.ChangeAttendanceStatus(request.timeTableId, request.attendenceStatusCode);
            if (msg.Length > 0) return Ok(msg);

            return Ok("Đã đổi trạng thái điểm danh");
        }
    }
}
