using FAP_API.ModelDTO;
using FAP_API.Models;
using FAP_API.ViewModels;

namespace FAP_API.Services
{
    public interface IUserService
    {
        User GetUserByUserName(string username);

        List<AccountViewModel> GetUserList();

        AccountDetail GetUserDetailByUserName(string username);
        bool UpdateUser(AccountDetail accountDetail);
        User GetUserName(string id);

        List<String> GetRoleList();

        public List<String?> GetStudentStatusList();
        public List<String?> GetTeacherStatusList();

    }

}
