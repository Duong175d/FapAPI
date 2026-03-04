
namespace FAP_API.ViewModels
{
    public class ExamUserViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string ExamId { get; set; }
        public string SubjectCode { get; set; }
        public string RoomName { get; set; }
        public string SlotTime { get; set; }
        public DateOnly? ExamDate { get; set; }
        public string Semester { get; set; }
        public string Note { get; set; }
        public TimeOnly? TimeStart { get; set; }
        public TimeOnly? TimeEnd { get; set; }
        public string SemesterName { get; set; }
    }


    public class ExamScheduleViewModel
    {
        public string SubjectCode { get; set; }
        public string RoomName { get; set; }
        public string SlotTime { get; set; }
        public DateOnly? ExamDate { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
    }


    public class ExamScheduleUpdateViewModel
    {
        public string EsId { get; set; }
        public string UserName { get; set; }
        public string SubjectCode { get; set; }
        public string RoomName { get; set; }
        public string SlotTime { get; set; }
        public DateOnly? ExamDate { get; set; }
        public string Note { get; set; }
        public int Type { get; set; }
    }

    public class SemesterVM
    {
        public DateOnly ExamDate { get; set; }
        public string SemesterName { get; set; }

        public static implicit operator SemesterVM(List<SemesterVM> v)
        {
            throw new NotImplementedException();
        }
    }

    public class ExamSchedulerVM
    {
        public string EsId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public string RoomName { get; set; }
        public string SlotName { get; set; }
        public int? SlotNumber { get; set; }
        public string UserName {  get; set; }
        public DateOnly? ExamDate { get; set; }
        public string SemesterName { get; set; }
        public TimeOnly? TimeStart { get; set; }
        public TimeOnly? TimeEnd { get; set; }
        public string Note { get; set; }
        public int? Type { get; set; }
    }

    public class RoomDto
    {
        public string RoomId { get; set; }
        public string Name { get; set; }
    }

    public class SubjectDto
    {
        public string SubjectId { get; set; }
        public string SubjecCode { get; set; }
    }

    public class LecturerDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
   

}
