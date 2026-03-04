using System;

namespace FAP_API.ViewModels
{
    public class TimetableViewModel
    {
        public string TimeTableId { get; set; }
        public string Date { get; set; }
        public string DayOfWeek { get; set; }
        public string SlotNumber { get; set; }
        public TimeOnly TimeStart { get; set; }
        public TimeOnly TimeEnd { get; set; }
        public string Room { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string ClassCode { get; set; }
        public string Lecturer { get; set; }
        public string AttendanceStatus { get; set; }
        public bool IsOnline { get; set; }
    }

    public class TimetableDayViewModel
    {
        public string Date { get; set; }
        public List<TimetableViewModel> TimetableViewModels { get; set; }
    }

    public class TimetableRequestViewModel
    {
        public string UserId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Semester { get; set; }
    }
    public class TimetableCreateAutoViewModel
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Semester { get; set; }
    }
}