namespace FAP_API.ViewModels
{
    public class AttendanceReportVM
    {
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string ClassName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int AttendancePercent { get; set; }
        public int SlotPresent { get; set; }
        public int SlotAbsent { get; set; }
    }

    public class AttendanceDetails
    {
        public string SubjectName { get; set; }
        public string ClassName { get; set; }
        public int AttendancePercent { get; set; }
        public int SlotPresent { get; set; }
        public int SlotAbsent { get; set; }
        public int SlotNotyet { get; set; }
        public List<AttendanceItem> AttendanceItems { get; set; } = new List<AttendanceItem>(); 
    }

    public class AttendanceItem
    {
        public string Date { get; set; }
        public string Slot { get; set; }
        public string Lecturer { get; set; }
        public int RollCallStatus { get; set; }

    }
}
