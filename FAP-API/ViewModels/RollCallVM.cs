namespace FAP_API.ViewModels
{
    public class RollCallVM
    {
        public string TimeTableId { get; set; }
        public string UserId { get; set; } = null!;
        public int? RollCallStatus { get; set; }
    }

    public class UserRollCallVM
    {
        public string UserId { get; set; } = null!;
        public string? StudentCode { get; set; }
        public string? FullName { get; set; }
        public int? RollCallStatus { get; set; }
    }
}
