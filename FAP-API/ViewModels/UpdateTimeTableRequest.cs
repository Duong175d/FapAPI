public class UpdateTimeTableRequest
{
    public string TimeTableId { get; set; }
    public string ClassId { get; set; }
    public string UserId { get; set; }
    public string SubjectId { get; set; }
    public string SlotId { get; set; }
    public string RoomId { get; set; }
    public DateOnly DateTime { get; set; }
    public string SemesterId { get; set; }
    public int IsAttendance { get; set; }
    public bool IsOnline { get; set; }
}
