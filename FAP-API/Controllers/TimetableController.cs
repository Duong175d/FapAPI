using FAP_API.Constants;
using FAP_API.Models;
using FAP_API.Services;
using FAP_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FAP_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimetableController : ControllerBase
    {
        private readonly ITimeTableService _timeTableService;

        public TimetableController(ITimeTableService timeTableService)
        {
            _timeTableService = timeTableService;
        }
        //[Authorize(Roles = ConstantsDescription.ROLE_STUDENT)]
        [HttpGet("weeklyStudent")]
        public IActionResult GetWeeklyTimetableForStudent([FromQuery] TimetableRequestViewModel request)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(request.UserId) || request.StartDate == default || request.EndDate == default || string.IsNullOrEmpty(request.Semester))
            {
                return BadRequest("Invalid input parameters.");
            }

            // Call service to get valid timetables
            var timetable = _timeTableService.GetValidTimetables(request);


            return Ok(new
            {
                Semester = _timeTableService.GetListSemesterWithSubject().FirstOrDefault(s => s.SemesterId == request.Semester).SemesterName,
                WeekRange = $"{request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}",
                Timetable = timetable
            });
        }

        //[Authorize(Roles = ConstantsDescription.ROLE_TEACHER)]
        [HttpGet("weeklyTeacher")]
        public IActionResult GetWeeklyTimetableForTeacher([FromQuery] TimetableRequestViewModel request)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(request.UserId) || request.StartDate == default || request.EndDate == default || string.IsNullOrEmpty(request.Semester))
            {
                return BadRequest("Invalid input parameters.");
            }

            // Call service to get valid timetables
            var timetable = _timeTableService.GetValidTimetablesLecturer(request);

            return Ok(new
            {
                Semester = _timeTableService.GetListSemesterWithSubject().FirstOrDefault(s => s.SemesterId == request.Semester).SemesterName,
                WeekRange = $"{request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}",
                Timetable = timetable,
            });
        }
        //[Authorize]
        [HttpGet("listClasses")]
        public IActionResult GetListClasses()
        {
            var classes = _timeTableService.GetListClass();
            if (classes == null || !classes.Any())
            {
                return NotFound("No classes found.");
            }

            return Ok(classes.Select(c => new
            {
                ClassId = c.ClassId,
                ClassCode = c.ClassCode
            }));
        }

        //[Authorize]
        [HttpGet("listSemestersWithSubjects")]
        public IActionResult GetListSemestersWithSubjects()
        {
            var semestersWithSubjects = _timeTableService.GetListSemesterWithSubject();
            if (semestersWithSubjects == null || !semestersWithSubjects.Any())
            {
                return NotFound("No semesters or subjects found.");
            }

            var result = semestersWithSubjects
                .Select(s => new
                {
                    SemesterId = s.SemesterId,
                    SemesterName = s.SemesterName,
                    Subjects = s.Subjects.Select(sub => new
                    {
                        SubjectId = sub.SubjectId,
                        SubjecCode = sub.SubjectName
                    }).ToList()
                })
                .OrderBy(s => s.SemesterName)
                .ToList();

            return Ok(result);
        }
        //[Authorize(Roles = ConstantsDescription.ROLE_ADMIN)]
        [HttpPost("createTimetableAutomatically")]
        public IActionResult CreateTimetableAutomatically([FromBody] CreateTimeTableRequest request)
        {
            string msg = "";
            if (request == null || string.IsNullOrEmpty(request.SemesterId) || string.IsNullOrEmpty(request.ClassId)
                || string.IsNullOrEmpty(request.SubjectId) || request.StartDate == default)
            {
                return Ok(new { status = "invalid_input" });
            }

            msg = _timeTableService.CreateTimetableAutomatically(request);
            if (msg.Length > 0) return Ok(msg);

            return Ok(new { status = "success" });
        }

        //[Authorize(Roles = ConstantsDescription.ROLE_ADMIN)]
        [HttpPut("updateTimetable")]
        public IActionResult UpdateTimetable([FromBody] UpdateTimeTableRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.TimeTableId) || string.IsNullOrEmpty(request.ClassId)
                || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.SubjectId)
                || string.IsNullOrEmpty(request.SlotId) || string.IsNullOrEmpty(request.RoomId)
                || request.DateTime == default || string.IsNullOrEmpty(request.SemesterId))
            {
                return Ok(new { status = "invalid_input" });
            }

            var msg = _timeTableService.UpdateTimetable(request);
            if (msg.Length > 0) return Ok(msg);

            return Ok(new { status = "success" });
        }

        //[Authorize(Roles = ConstantsDescription.ROLE_ADMIN)]
        [HttpPut("deleteTimetable/{timetableId}")]
        public IActionResult DeleteTimetable(string timetableId)
        {
            if (string.IsNullOrEmpty(timetableId))
                return Ok(new { status = "invalid_input" });

            var msg = _timeTableService.DeleteTimetable(timetableId);
            if (msg.Length > 0) return Ok(msg);

            return Ok(new { status = "success" });
        }

        [HttpGet("studentsInSlot")]
        public IActionResult StudentInSlot([FromQuery]string timetableId)
        {
            if (string.IsNullOrEmpty(timetableId))
                return Ok("Invalid timetable ID.");
            var result = _timeTableService.GetStudentInSlot(timetableId);
            return Ok(result);
        }
        [HttpPost("RollCallStudent")]
        public IActionResult RollCallStudent([FromBody] List<RollCallVM> rollCallVM)
        {
            if (rollCallVM == null) return Ok(new { status = "error"});
            string result = _timeTableService.RollCallStudent(rollCallVM);

            return Ok(new { status = result });
        }
        [HttpGet("AttendanceReport")]
        public IActionResult AttendanceReport([FromQuery]string userId) 
        {
            var result = _timeTableService.GetAttendanceReportVM(userId);
            return Ok(result);
        }

        [HttpGet("AttendanceDetails")]
        public IActionResult AttendanceDetails([FromQuery] string userId, string subjectId)
        {
            var result = _timeTableService.GetAttendanceDetails(userId, subjectId);
            return Ok(result);
        }
        [HttpGet("listClassUsers")]
        public IActionResult GetListClassUser()
        {
            var classUsers = _timeTableService.GetListClassWithStudent();
            if (classUsers == null || !classUsers.Any())
            {
                return Ok(new { status = "notfound" });
            }

            var result = classUsers.Select(cu => new
            {
                ClassId = cu.ClassId,
                ClassCode = cu.ClassCode,
                Users = cu.Users.Select(u => new
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    StudentCode = u.StudentCode
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("listTeacher")]
        public IActionResult GetListTeacher()
        {
            var teachers = _timeTableService.GetListTeacher();
            if (teachers == null || !teachers.Any())
            {
                return Ok(new { status = "notfound" });
            }
            var result = teachers.Select(t => new
            {
                UserId = t.UserId,
                FullName = t.FullName,
                UserName = t.UserName
            }).ToList();
            return Ok(result);
        }
    }
}