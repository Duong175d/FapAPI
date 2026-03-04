namespace FAP_API.ViewModels
{
	public class LetterViewModel
	{
		public string Type { get; set; }
		public string Reason { get; set; }
		public string? CodeSubject { get; set; }
	}
	public class GetLetterForTestingViewModel
	{
		public string LetterId { get; set; }
		public string Status { get; set; }
		public string Type { get; set; }
		public string CreateDate { get; set; }
		public string FullName { get; set; }
		public string StudentCode { get; set; }

	}

	public class GetLetterForTestingResponseViewModel
	{
		public string LetterId { get; set; }
		public string Status { get; set; }
		public string Type { get; set; }
		public string CreateDate { get; set; }
		public string FullName { get; set; }
		public string StudentCode { get; set; }
		public string CodeSubject { get; set; }
		public string Reason { get; set; }
		public string Response {  get; set; }

	}

	public class LetterGetByUserIdViewModel
	{
		public string Type { get; set; }
		public string Reason { get; set; }
		public string Status { get; set; }
		public string CreateDate { get; set; }
        public string Response { get; set; }
    }

	public class UserInfoForSendLetterViewModel
	{
		public string FullName { get; set; }
		public string StudentCode { get; set; }
	}
}