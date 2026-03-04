using FAP_API.Constants;
using FAP_API.Models;

namespace FAP_API.Services
{
    public interface IAttendenceService
    {
        string GetListAttendanceStatus(out List<SystemKey> attendanceStatuses);
        string ChangeAttendanceStatus(string timetableId, int statusCode);
        TimeTable GetTimeTableById(string timetableId);
    }
    public class AttendenceService : IAttendenceService
    {
        private readonly Prm392Context _context;
        public AttendenceService(Prm392Context context)
        {
            _context = context;
        }

        public string ChangeAttendanceStatus(string timetableId, int statusCode)
        {
            string msg = "";
            try
            {
                var timetable = _context.TimeTables
                    .FirstOrDefault(t => t.TimeTableId == timetableId);
                if (timetable == null)
                {
                    msg = "Không tìm thấy lịch học với ID đã cho.";
                    return msg;
                }
                timetable.IsAttendance = statusCode;
                _context.TimeTables.Update(timetable);
                _context.SaveChanges();
                return msg;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string GetListAttendanceStatus(out List<SystemKey> attendanceStatuses)
        {
            attendanceStatuses = new List<SystemKey>();
            string msg = "";
            try {
                attendanceStatuses = _context.SystemKeys
                    .Where(s => s.ParrentId == ConstantsValue.ROLL_CALL_STATUS).ToList();
                if (attendanceStatuses == null)
                {
                    msg = "Không có trạng thái điểm danh nào.";
                    return msg;
                }
                return msg;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public TimeTable GetTimeTableById(string timetableId)
        {
            return _context.TimeTables
                .FirstOrDefault(t => t.TimeTableId == timetableId) ?? new TimeTable();
        }
    }
}
