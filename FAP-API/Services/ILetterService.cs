using FAP_API.Models;
using FAP_API.ViewModels;

namespace FAP_API.Services
{
	public interface ILetterService
	{
		List<GetLetterForTestingViewModel> GetLetterList();
		List<LetterGetByUserIdViewModel> GetLettersByUserId(string userId);
		List<GetLetterForTestingViewModel> GetLettersByStatus(string status);
		List<GetLetterForTestingViewModel> GetLettersByType(string type);
		GetLetterForTestingResponseViewModel GetLetterByLetterId(string letterId);
		bool UpdateLetterStatus(string letterId, string newStatus);
		bool UpdateLetterResponse(string letterId, string response);
		void CreateLetter(string userId, LetterViewModel viewModel);
		List<string> GetTypeLetterDescriptions();
		UserInfoForSendLetterViewModel GetInfoByUserId(string userId);

	}
}