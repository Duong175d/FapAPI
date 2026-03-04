using FAP_API.Models;
using FAP_API.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FAP_API.Services
{
    public interface INotificationService
    {
        public List<NotificationVM> GetList(string? search);
        public Notification GetDetaiById(string notificationId);
    }
    public class NotificationService : INotificationService
    {
        private readonly Prm392Context _context;

        public NotificationService(Prm392Context context)
        {
            _context = context;
        }

        public Notification GetDetaiById(string notificationId)
        {
            var notification = _context.Notifications.Where(e => e.NotificationId == notificationId).FirstOrDefault();

            if(notification == null)
            {
                throw new Exception("Không tồn tại");
            }
            return notification;
        }

        public List<NotificationVM> GetList(string? search)
        {
            var ltNotification = new List<NotificationVM>();
            if (search.IsNullOrEmpty())
            {
                ltNotification = _context.Notifications
              .Select(ex => new NotificationVM
              {
                  NotificationId = ex.NotificationId,
                  Title = ex.Title,
                  Date = ex.Date.HasValue ? ex.Date.Value.ToString("d/M/yyyy") : ""
              })
              .ToList();
            }
            else
            {
                ltNotification = _context.Notifications.Where(e => e.Title.Contains(search))
                   .Select(ex => new NotificationVM
                   {
                       NotificationId = ex.NotificationId,
                       Title = ex.Title,
                       Date = ex.Date.HasValue ? ex.Date.Value.ToString("d/M/yyyy") : ""
                   })
                   .ToList();
                if (ltNotification == null)
                {
                    throw new Exception("Lỗi lấy dữ liệu");
                }
            }
            return ltNotification;
        }
    }
}
