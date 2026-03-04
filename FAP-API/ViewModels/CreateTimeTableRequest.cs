namespace FAP_API.ViewModels
{
    public class CreateTimeTableRequest
    {
        public DateOnly StartDate { get; set; }
        public string SemesterId { get; set; }
        public string ClassId { get; set; }
        public string SubjectId { get; set; }
    }
}
