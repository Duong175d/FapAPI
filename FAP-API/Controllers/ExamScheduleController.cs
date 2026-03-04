using FAP_API.Models;
using FAP_API.Services;
using FAP_API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FAP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamScheduleController : ControllerBase
    {
        private readonly IExamScheduleService examScheduleService;

        public ExamScheduleController(IExamScheduleService _examScheduleService)
        {
            examScheduleService = _examScheduleService;
        }

        [HttpGet]
        public IActionResult GetExams(string userId, string semesterName)
        {
            var timetables = examScheduleService.GetExam(userId, semesterName);
            return Ok(timetables);
        }


        [HttpGet("GetExamAmin")]
        public IActionResult GetExamAmin(string semesterName)
        {
            var timetables = examScheduleService.GetExamAmin(semesterName);
            return Ok(timetables);
        }


        [HttpGet("GetAllRooms")]
        public IActionResult GetAllRooms()
        {
            var list = examScheduleService.GetAllRooms();
            return Ok(list);
        }

        [HttpGet("GetAllSubjects")]
        public IActionResult GetAllSubjects()
        {
            var list = examScheduleService.GetAllSubjects();
            return Ok(list);
        }

        [HttpGet("AllLecturer")]
        public IActionResult GetAllLecturer()
        {
            var list = examScheduleService.GetAllLecturer();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult CreateExam([FromBody] ExamScheduleViewModel examSchedule)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                examScheduleService.CreateExamSchedule(examSchedule);
                return Ok(new { message = "Created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult UpdateExam([FromBody] ExamScheduleUpdateViewModel examSchedule)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                examScheduleService.UpdateExamSchedule(examSchedule);
                return Ok(new { message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteExam(string id)
        {
            try
            {
                var examToDelete = new ExamSchedule { EsId = id };
                examScheduleService.DeleteExamSchedule(examToDelete);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("GetUsersByExamId")]
        public IActionResult GetUsersByExamId(string examId)
        {
            var ltUser = examScheduleService.getUserById(examId);
            return Ok(ltUser);
        }

        [HttpGet("student/semesters")]
        public IActionResult GetStudentSemesters([FromQuery] string userId)
        {
            try
            {
                var result = examScheduleService.GetStudentSemesters(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("LectureSemesters")]
        public IActionResult GetLectureSemesters()
        {
            try
            {
                var result = examScheduleService.GetSemesterLecture();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EsById")]
        public IActionResult GetEsById(string es)
        {
            try
            {
                var result = examScheduleService.GetExamById(es);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
