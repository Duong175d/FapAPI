using FAP_API.ModelDTO;
using FAP_API.Models;
using FAP_API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FAP_API.Services
{
    public class UserService(Prm392Context _prm392Context) : IUserService
    {
        public User GetUserByUserName(string username)
        {
            return _prm392Context.Users.FirstOrDefault(u => u.UserName == username);
        }

        public AccountDetail? GetUserDetailByUserName(string username)
        { 
            return _prm392Context.Users
                    .Where(u => u.UserName == username)
                    .Select(u => new AccountDetail
                    {
                        Email = u.Email,
                        Role = _prm392Context.SystemKeys.FirstOrDefault(s => s.Id == u.Role).Description,
                        StudentCode = u.StudentCode,
                        FullName = u.FullName,
                        Status = _prm392Context.SystemKeys.FirstOrDefault(s => s.Id == u.Status).Description 
                    })
                    .FirstOrDefault();
        }

        public List<AccountViewModel> GetUserList()
        {
            return _prm392Context.Users.Select(u => new AccountViewModel
            {
                Username = u.UserName,
                Role = _prm392Context.SystemKeys.FirstOrDefault(s => s.Id == u.Role).Description,
                Status = _prm392Context.SystemKeys.FirstOrDefault(s => s.Id == u.Status).Description
            }).ToList();
        }

        public bool UpdateUser(AccountDetail accountDetail)
        {
            var user = _prm392Context.Users.FirstOrDefault(u => u.Email == accountDetail.Email);
            if (user == null) return false;

            user.FullName = accountDetail.FullName;
            user.StudentCode = accountDetail.StudentCode;
            user.Role = _prm392Context.SystemKeys.FirstOrDefault(s => s.Description == accountDetail.Role).Id;
            if (accountDetail.Status == null)
            {
                user.Status = null;
            }
            user.Status = _prm392Context.SystemKeys.FirstOrDefault(s => s.Description == accountDetail.Status).Id; 

            _prm392Context.SaveChanges();
            return true;
        }

        public User GetUserName(string id)
        {
            return _prm392Context.Users.FirstOrDefault(u => u.UserId == id);
        }

        public List<String?> GetRoleList()
        {
            return _prm392Context.SystemKeys
            .Where(k => k.ParrentId == 1)
            .Select(d => d.Description)
            .ToList();
        }

        public List<String?> GetStudentStatusList()
        {
            return _prm392Context.SystemKeys
            .Where(k => k.ParrentId == 37)
             .Select(d => d.Description)
            .ToList();
        }
        public List<String?> GetTeacherStatusList()
        {
            return _prm392Context.SystemKeys
            .Where(k => k.ParrentId == 39)
             .Select(d => d.Description)
            .ToList();
        }
    }

}
