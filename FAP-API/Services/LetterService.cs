using FAP_API.Models;
using FAP_API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FAP_API.Services
{
	public class LetterService : ILetterService
	{
		private readonly Prm392Context _context;

		public LetterService(Prm392Context context)
		{
			_context = context;
		}

		public void CreateLetter(string userId, LetterViewModel viewModel)
		{
			var newLetter = new Letter
			{
				LetterId = Guid.NewGuid().ToString(),
				UserId = userId,
				Type = _context.SystemKeys.FirstOrDefault(s => s.Description == viewModel.Type).Id,
				Reason = viewModel.Reason,
				CodeSubject = viewModel.CodeSubject,
				CreateDate = DateTime.UtcNow,
				Status = _context.SystemKeys.FirstOrDefault(s => s.CodeKey == "UNREAD").Id,
				IsDelete = false
			};

			_context.Letters.Add(newLetter);
			_context.SaveChanges();
		}

		public List<GetLetterForTestingViewModel> GetLetterList()
		{
			var letter = _context.Letters.ToList();
			var letterResponse = new List<GetLetterForTestingViewModel>();

			for (int i = 0; i < letter.Count(); i++)
			{
				letterResponse.Add(new GetLetterForTestingViewModel
				{
					LetterId = letter[i].LetterId,
					Type = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Type).Description,
					CreateDate = letter[i].CreateDate?.Date.ToString("dd/MM/yyyy"),
					Status = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Status).Description,
					StudentCode = _context.Users.FirstOrDefault(u => u.UserId == letter[i].UserId).StudentCode,
					FullName = _context.Users.FirstOrDefault(u => u.UserId == letter[i].UserId).FullName
				});
			}
			return letterResponse;
		}

		public List<GetLetterForTestingViewModel> GetLettersByStatus(string status)
		{

			if (status == "Unread" || status == "In Process")
			{
				var statusSystem = _context.SystemKeys.FirstOrDefault(s => s.Description == status).Id;
				var letter = _context.Letters.Where(l => l.Status == statusSystem).ToList();

				var letterResponse = new List<GetLetterForTestingViewModel>();

				for (int i = 0; i < letter.Count(); i++)
				{
					letterResponse.Add(new GetLetterForTestingViewModel
					{
						LetterId = letter[i].LetterId,
						Type = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Type).Description,
						CreateDate = letter[i].CreateDate?.Date.ToString("dd/MM/yyyy"),
						Status = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Status).Description,
						StudentCode = _context.Users.FirstOrDefault(u => u.UserId == letter[i].UserId).StudentCode,
						FullName = _context.Users.FirstOrDefault(u => u.UserId == letter[i].UserId).FullName
					});
				}
				return letterResponse;
			} else
			{
				var statusList = new List<string> { "Reject", "Approved" };
				var statusIds = _context.SystemKeys
					.Where(s => statusList.Contains(s.Description))
					.Select(s => s.Id)
					.ToList();

				var letter = _context.Letters
					.Where(l => statusIds.Contains(l.Status.Value))
					.ToList();

				var letterResponse = new List<GetLetterForTestingViewModel>();

				for (int i = 0; i < letter.Count(); i++)
				{
					letterResponse.Add(new GetLetterForTestingViewModel
					{
						LetterId = letter[i].LetterId,
						Type = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Type).Description,
						CreateDate = letter[i].CreateDate?.Date.ToString("dd/MM/yyyy"),
						Status = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Status).Description,
						StudentCode = _context.Users.FirstOrDefault(u => u.UserId == letter[i].UserId).StudentCode,
						FullName = _context.Users.FirstOrDefault(u => u.UserId == letter[i].UserId).FullName
					});
				}
				return letterResponse;
			}
		}

		public List<LetterGetByUserIdViewModel> GetLettersByUserId(string userId)
		{
			var letter = _context.Letters.Where(l => l.UserId == userId).ToList();
			var letterResponse = new List<LetterGetByUserIdViewModel>();
			for(int i = 0; i < letter.Count(); i++)
			{
				letterResponse.Add(new LetterGetByUserIdViewModel
				{
					Type = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Type).Description,
					Reason = letter[i].Reason,
					CreateDate = letter[i].CreateDate?.Date.ToString("dd/MM/yyyy"),
					Status = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Status).Description,
					Response = letter[i].Response
				});
			}
			return letterResponse;
		}

		public bool UpdateLetterResponse(string letterId, string response)
		{
			var letter = _context.Letters.FirstOrDefault(l => l.LetterId == letterId);
			if (letter == null) return false;

			letter.Response = response;
			_context.SaveChanges();
			return true;
		}

		public bool UpdateLetterStatus(string letterId, string newStatus)
		{
			var letter = _context.Letters.FirstOrDefault(l => l.LetterId == letterId);
			if (letter == null) return false;

			letter.Status = _context.SystemKeys.FirstOrDefault(s => s.Description == newStatus).Id;
			_context.SaveChanges();
			return true;
		}

		private int getIdTypeLetter()
		{
			return _context.SystemKeys.FirstOrDefault(s => s.CodeKey == "TYPE_LETTER").Id;
		}

		public List<string> GetTypeLetterDescriptions()
		{
			int parrentId = getIdTypeLetter();
			return _context.SystemKeys
				.Where(s => s.ParrentId == parrentId)
				.Select(s => s.Description)
				.ToList();
		}

		public UserInfoForSendLetterViewModel GetInfoByUserId(string userId)
		{
			return _context.Users
			.Where(u => u.UserId == userId)
			.Select(u => new UserInfoForSendLetterViewModel
				{
				FullName = u.FullName,
				StudentCode = u.StudentCode
				})
				.FirstOrDefault();
		}

		public GetLetterForTestingResponseViewModel GetLetterByLetterId(string letterId)
		{
			var letter = _context.Letters.FirstOrDefault(l => l.LetterId == letterId);
			var letterResponse = new GetLetterForTestingResponseViewModel();

			letterResponse.LetterId = letter.LetterId;
			letterResponse.Status = _context.SystemKeys.FirstOrDefault(s => s.Id == letter.Status).Description;
			letterResponse.Type = _context.SystemKeys.FirstOrDefault(s => s.Id == letter.Type).Description;
			letterResponse.CreateDate = letter.CreateDate?.Date.ToString("dd/MM/yyyy");
			letterResponse.FullName = _context.Users.FirstOrDefault(u => u.UserId == letter.UserId).FullName;
			letterResponse.StudentCode = _context.Users.FirstOrDefault(u => u.UserId == letter.UserId).StudentCode;
			letterResponse.CodeSubject = letter.CodeSubject;
			letterResponse.Reason = letter.Reason;
			letterResponse.Response = letter.Response;

			return letterResponse;
		}

		public List<GetLetterForTestingViewModel> GetLettersByType(string type)
		{
			var typeSystem = _context.SystemKeys.FirstOrDefault(s => s.Description == type).Id;
			var letter = _context.Letters.Where(l => l.Type == typeSystem).ToList();

			var letterResponse = new List<GetLetterForTestingViewModel>();

			for (int i = 0; i < letter.Count(); i++)
			{
				letterResponse.Add(new GetLetterForTestingViewModel
				{
					LetterId = letter[i].LetterId,
					Type = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Type).Description,
					CreateDate = letter[i].CreateDate?.Date.ToString("dd/MM/yyyy"),
					Status = _context.SystemKeys.FirstOrDefault(s => s.Id == letter[i].Status).Description,
					StudentCode = _context.Users.FirstOrDefault(u => u.UserId == letter[i].UserId).StudentCode,
					FullName = _context.Users.FirstOrDefault(u => u.UserId == letter[i].UserId).FullName
				});
			}
			return letterResponse;
		}
	}
}
