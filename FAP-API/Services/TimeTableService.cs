using FAP_API.Constants;
using FAP_API.Models;
using FAP_API.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace FAP_API.Services
{
    public interface ITimeTableService
    {
        List<TimetableDayViewModel> GetValidTimetables(TimetableRequestViewModel request);
        public List<TimetableDayViewModel> GetValidTimetablesLecturer(TimetableRequestViewModel request);
        string CreateTimetableAutomatically(CreateTimeTableRequest request);
        string UpdateTimetable(UpdateTimeTableRequest request);
        string DeleteTimetable(string timetableId);
        List<Class> GetListClass();
        List<Semester> GetListSemesterWithSubject();
        List<Room> GetListRoom();
        List<Slot> GetListSlot();
        List<User> GetListLecturer();
        public List<UserRollCallVM> GetStudentInSlot(string slotId);
        public string RollCallStudent(List<RollCallVM> rollCallVM);
        List<ClassUser> GetListClassWithStudent();
        public List<AttendanceReportVM> GetAttendanceReportVM(string userId);
        public AttendanceDetails GetAttendanceDetails(string userId, string subjectId);
        public List<User> GetListTeacher();
    }
    public class TimeTableService : ITimeTableService
    {
        private readonly Prm392Context _context;

        public TimeTableService(Prm392Context context)
        {
            _context = context;
        }

        public List<TimetableDayViewModel> GetValidTimetables(TimetableRequestViewModel request)
        {
            List<TimetableDayViewModel> result = new List<TimetableDayViewModel> ();
            var timetable = _context.TimeTables
                .Include(t => t.Slot)
                .Include(t => t.User)
                .Include(t => t.Room)
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .Include(t => t.UserTimeTables)
                    //.ThenInclude(c => c.Users)
                .Where(t => t.Class.Users.Any(u => u.UserId == request.UserId)
                            && t.Semester == request.Semester
                            && t.DateTime >= request.StartDate
                            && t.DateTime <= request.EndDate
                            && !t.IsDelete
                            )

                .OrderBy(t => t.DateTime)

                .ThenBy(t => t.Slot.SlotName)
                .AsEnumerable()
                .ToList(); 
            DateOnly mondayOffWeek = timetable[0].DateTime; 

            while (mondayOffWeek.DayOfWeek > DayOfWeek.Monday)
            {
                mondayOffWeek = mondayOffWeek.AddDays(-1);
            }
            List<DateOnly> dateOfWeek = new List<DateOnly>();
            for (int i = 0; i < 7; i++)
            {
                dateOfWeek.Add(mondayOffWeek);
                mondayOffWeek = mondayOffWeek.AddDays(1);
            }
            foreach (var t in dateOfWeek)
            {
                TimetableDayViewModel addToResult = new TimetableDayViewModel();
                var listTimeTable = timetable.Where(x => x.DateTime == t)
                    .Select(t => new TimetableViewModel
                    {
                        Date = t.DateTime.ToString("dd/MM/yyyy"),
                        DayOfWeek = t.DateTime.ToString("dddd"),
                        SlotNumber = t.Slot.SlotName,
                        TimeStart = (TimeOnly)t.Slot.TimeStart,
                        TimeEnd = (TimeOnly)t.Slot.TimeEnd,
                        Lecturer = t.User.UserName,
                        Room = t.Room.Name,
                        SubjectCode = t.Subject.SubjectCode,
                        SubjectName = t.Subject.SubjectName,
                        ClassCode = t.Class.ClassCode,
                        AttendanceStatus = 
                            t.UserTimeTables.FirstOrDefault(u => u.UserId == request.UserId && u.TimeTableId == t.TimeTableId).RollCallStatus == ConstantsValue.ROLL_CALL_PRESENT 
                            ? ConstantsDescription.ROLL_CALL_PRESENT : 
                            t.UserTimeTables.FirstOrDefault(u => u.UserId == request.UserId && u.TimeTableId == t.TimeTableId).RollCallStatus == ConstantsValue.ROLL_CALL_ABSENT
                            ? ConstantsDescription.ROLL_CALL_ABSENT :
                            t.UserTimeTables.FirstOrDefault(u => u.UserId == request.UserId && u.TimeTableId == t.TimeTableId).RollCallStatus == ConstantsValue.ROLL_CALL_NOT_YET
                            ? ConstantsDescription.ROLL_CALL_NOT_YET : "Unkown",
                        IsOnline = t.IsOnline ?? false
                    }).ToList();
                if (listTimeTable.Count > 0)
                {
                    addToResult = new TimetableDayViewModel
                    {
                        Date = t.ToString("dd/MM/yyyy"),
                        TimetableViewModels = listTimeTable,

                    };
                }
                else {
                    addToResult = new TimetableDayViewModel
                    {
                        Date = t.ToString("dd/MM/yyyy"),
                        TimetableViewModels = new List<TimetableViewModel>()
                    };
                }

                result.Add(addToResult);
            }

            return result;
        }

        public string CreateTimetableAutomatically(CreateTimeTableRequest request)
        {
            string msg = "";
            try
            {
                // Kiểm tra xem các subjectId có thuộc kỳ học theo semesterId hay không
                var semester = _context.Semesters
                    .Include(s => s.Subjects)
                    .FirstOrDefault(s => s.SemesterId == request.SemesterId);

                //Lấy học sinh trong lớp
                var students = _context.Classes
                    .Where(c => c.ClassId == request.ClassId)
                    .Select(c => c.Users).ToList();

                string className = _context.Classes.FirstOrDefault(c => c.ClassId == request.ClassId).ClassCode;

                if (semester == null)
                {
                    throw new Exception("Không tìm thấy kỳ học hoặc không có môn học nào trong kỳ học.");
                }

                var subjectToSchedule = semester.Subjects.FirstOrDefault(s => s.SubjectId == request.SubjectId);

                if (subjectToSchedule == null)
                    throw new Exception("Môn học được yêu cầu không thuộc kỳ học đã chọn hoặc không tồn tại.");
                if (subjectToSchedule.NumberSlot == null || subjectToSchedule.NumberSlot < 10)
                    throw new Exception($"Môn học {subjectToSchedule.SubjectName} không có số slot quy định hoặc số slot không hợp lệ.");

                // Get all lecturers and rooms
                var allLecturers = GetListLecturer();
                var allRooms = GetListRoom();
                var allSlots = GetListSlot();

                if (!allLecturers.Any()) throw new Exception("Không tìm thấy giáo viên nào.");
                if (!allRooms.Any()) throw new Exception("Không tìm thấy phòng học nào.");
                if (allSlots.Count < 4) throw new Exception("Không đủ slot học để tạo thời khóa biểu.");

                // Chọn cặp slot cho môn học: 1-2 hoặc 3-4 (ví dụ random, hoặc theo yêu cầu)
                var slotPair1 = allSlots.Where(s => s.SlotNumber == 1 || s.SlotNumber == 2).OrderBy(s => s.SlotNumber).ToList();
                var slotPair2 = allSlots.Where(s => s.SlotNumber == 3 || s.SlotNumber == 4).OrderBy(s => s.SlotNumber).ToList();

                // Bạn có thể chọn cặp slot theo ý muốn, ví dụ random hoặc mặc định
                var slotPair = (new Random().Next(2) == 0) ? slotPair1 : slotPair2;

                int totalSlots = subjectToSchedule.NumberSlot.Value;
                int slotsCreated = 0;

                // Tạo danh sách các ngày trong tuần (thứ 2 đến thứ 6)
                var weekdays = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

                DateTime currentDate = request.StartDate.ToDateTime(TimeOnly.MinValue);
                var timetableEntries = new List<TimeTable>();
                var timeTableUserEntries = new List<UserTimeTable>();
                List<int> selectedDays = null;

                string lecturerId = string.Empty;
                string roomId = string.Empty;
                while (slotsCreated < totalSlots)
                {
                    // Tìm ngày bắt đầu tuần (thứ 2)
                    while (currentDate.DayOfWeek != DayOfWeek.Monday)
                        currentDate = currentDate.AddDays(1);

                    // Lưu các ngày trong tuần này
                    var weekDates = weekdays.Select(dow => currentDate.Date.AddDays((int)dow - (int)DayOfWeek.Monday)).ToList();

                    // Nếu là tuần đầu tiên, chọn ngẫu nhiên 2 ngày cách nhau 1-2 ngày
                    if (slotsCreated == 0)
                    {
                        var random = new Random();
                        int firstDayIndex = random.Next(0, weekdays.Length - 2); // Chọn ngày đầu tiên
                        int secondDayIndex = firstDayIndex + random.Next(1, 3); // Cách 1-2 ngày

                        // Lưu lại ngày đã chọn
                        selectedDays = new List<int> { firstDayIndex, secondDayIndex };
                    }

                    // Áp dụng ngày đã chọn cho tuần hiện tại
                    for (int i = 0; i < Math.Min(2, totalSlots - slotsCreated); i++)
                    {
                        int dayIndex = selectedDays[i];
                        var date = weekDates[dayIndex];

                        

                        if(slotsCreated == 0)
                        {
                            var notAvailabelLecture = _context.TimeTables
                            .Where(x => x.DateTime== DateOnly.FromDateTime(date) && x.SlotId == slotPair[i].SlotId)
                            .Select(x => x.UserId)
                            .ToList();

                            var notAvailabelRoom = _context.TimeTables
                                .Where(x => x.DateTime == DateOnly.FromDateTime(date) && x.SlotId == slotPair[i].SlotId)
                                .Select(x => x.RoomId)
                                .ToList();

                            // Chọn giáo viên và phòng học bất kỳ hợp lệ
                            var availableLecturer = allLecturers
                                .Where(l => l.Subjects.Any(s => s.SubjectId == subjectToSchedule.SubjectId) && !notAvailabelLecture.Contains(l.UserId))
                                .OrderBy(u => u.UserName)
                                .FirstOrDefault();

                            var availableRoom = allRooms
                                .Where(r => !notAvailabelRoom.Contains(r.RoomId))
                                .OrderBy(r => r.Name).FirstOrDefault();

                            lecturerId = availableLecturer.UserId;
                            roomId = availableRoom.RoomId;

                        }

                        if (lecturerId != string.Empty && roomId != string.Empty)
                        {
                            var TimeTableId = Guid.NewGuid().ToString();
                            timetableEntries.Add(new TimeTable
                            {
                                TimeTableId = TimeTableId,
                                ClassId = request.ClassId,
                                UserId = lecturerId,
                                SubjectId = subjectToSchedule.SubjectId,
                                SlotId = slotPair[i].SlotId,
                                RoomId = roomId,
                                DateTime = DateOnly.FromDateTime(date),
                                Semester = request.SemesterId,
                                IsAttendance = ConstantsValue.ROLL_CALL_NOT_YET,
                                IsOnline = false,
                                IsDelete = false
                            });
                            slotsCreated++;
                            foreach (var student in students.SelectMany(c => c))
                            {
                                timeTableUserEntries.Add(new UserTimeTable
                                {
                                    TimeTableId = TimeTableId,
                                    RollCallStatus = ConstantsValue.ROLL_CALL_NOT_YET,
                                    UserId = student.UserId,
                                    SubjectId = request.SubjectId,
                                    Date = DateOnly.FromDateTime(date),
                                    ClassName = className,
                                    SubjectName = subjectToSchedule.SubjectName,
                                });
                            }
                        }
                    }

                    // Sang tuần sau
                    currentDate = currentDate.AddDays(7);
                }
                _context.UserTimeTables.AddRange(timeTableUserEntries);
                _context.TimeTables.AddRange(timetableEntries);
                _context.SaveChanges();
                return msg;
            }
            catch (Exception ex)
            {
                return $"Lỗi khi tạo thời khóa biểu tự động: {ex.Message}";
            }
        }

        public List<Class> GetListClass()
        {
            return _context.Classes.ToList();
        }

        public List<Semester> GetListSemesterWithSubject()
        {
            return _context.Semesters
                            .Include(s => s.Subjects)
                            .OrderBy(s => s.SemesterName)
                            .ToList();
        }

        public List<Room> GetListRoom()
        {
            return _context.Rooms.ToList();
        }

        public List<Slot> GetListSlot()
        {
            return _context.Slots.OrderBy(s => s.SlotNumber).ToList();
        }

        public List<User> GetListLecturer()
        {
            return _context.Users
                .Include(u => u.Subjects)
                .Where(u => u.Role == ConstantsValue.ROLE_TEACHER && u.IsDelete == false)
                .ToList();
        }
        public string UpdateTimetable(UpdateTimeTableRequest request)
        {
            string msg = "";
            try
            {
                // Find the existing timetable entry
                var existingTimetable = _context.TimeTables
                    .FirstOrDefault(t => t.TimeTableId == request.TimeTableId);

                if (existingTimetable == null)
                {
                    throw new Exception("Không tìm thấy thời khóa biểu cần cập nhật.");
                }

                // Validate the new data
                var semester = _context.Semesters
                    .Include(s => s.Subjects)
                    .FirstOrDefault(s => s.SemesterId == request.SemesterId);

                if (semester == null)
                {
                    throw new Exception("Không tìm thấy kỳ học hoặc không có môn học nào trong kỳ học.");
                }

                var subjectToUpdate = semester.Subjects.FirstOrDefault(s => s.SubjectId == request.SubjectId);

                if (subjectToUpdate == null)
                {
                    throw new Exception("Môn học được yêu cầu không thuộc kỳ học đã chọn hoặc không tồn tại.");
                }

                var allLecturers = GetListLecturer();
                var allRooms = GetListRoom();
                var allSlots = GetListSlot();

                var availableLecturer = allLecturers
                    .FirstOrDefault(l => l.UserId == request.UserId && l.Subjects.Any(s => s.SubjectId == subjectToUpdate.SubjectId));

                var availableRoom = allRooms.FirstOrDefault(r => r.RoomId == request.RoomId);

                var availableSlot = allSlots.FirstOrDefault(s => s.SlotId == request.SlotId);

                if (availableLecturer == null)
                {
                    throw new Exception("Không tìm thấy giáo viên hợp lệ.");
                }

                if (availableRoom == null)
                {
                    throw new Exception("Không tìm thấy phòng học hợp lệ.");
                }

                if (availableSlot == null)
                {
                    throw new Exception("Không tìm thấy slot học hợp lệ.");
                }

                // Update the timetable entry
                existingTimetable.ClassId = request.ClassId;
                existingTimetable.UserId = request.UserId;
                existingTimetable.SubjectId = request.SubjectId;
                existingTimetable.SlotId = request.SlotId;
                existingTimetable.RoomId = request.RoomId;
                existingTimetable.DateTime = request.DateTime;
                existingTimetable.Semester = request.SemesterId;
                existingTimetable.IsAttendance = request.IsAttendance;
                existingTimetable.IsOnline = request.IsOnline;

                _context.TimeTables.Update(existingTimetable);
                _context.SaveChanges();
                return msg;
            }
            catch (Exception ex)
            {
                return $"Lỗi khi cập nhật thời khóa biểu: {ex.Message}";
            }
        }

        public string DeleteTimetable(string timetableId)
        {
            try
            {
                var timetable = _context.TimeTables.FirstOrDefault(t => t.TimeTableId == timetableId);
                if (timetable == null)
                {
                    return "Không tìm thấy thời khóa biểu cần xóa.";
                }
                timetable.IsDelete = true;
                // Xóa thời khóa biểu
                _context.TimeTables.Update(timetable);
                _context.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi xóa thời khóa biểu: {ex.Message}";
            }
        }

        public List<TimetableDayViewModel> GetValidTimetablesLecturer(TimetableRequestViewModel request)
        {
            List<TimetableDayViewModel> result = new List<TimetableDayViewModel>();
            var timetable = _context.TimeTables
                .Include(t => t.Slot)
                .Include(t => t.User)
                .Include(t => t.Room)
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .Include(t => t.UserTimeTables)
                //.ThenInclude(c => c.Users)
                .Where(t => t.UserId == request.UserId
                            && t.Semester == request.Semester
                            && t.DateTime >= request.StartDate
                            && t.DateTime <= request.EndDate
                            && !t.IsDelete
                            )

                .OrderBy(t => t.DateTime)

                .ThenBy(t => t.Slot.SlotName)
                .AsEnumerable()
                .ToList();
            DateOnly mondayOffWeek = timetable[0].DateTime;

            while (mondayOffWeek.DayOfWeek > DayOfWeek.Monday)
            {
                mondayOffWeek = mondayOffWeek.AddDays(-1);
            }
            List<DateOnly> dateOfWeek = new List<DateOnly>();
            for (int i = 0; i < 7; i++)
            {
                dateOfWeek.Add(mondayOffWeek);
                mondayOffWeek = mondayOffWeek.AddDays(1);
            }
            foreach (var t in dateOfWeek)
            {
                TimetableDayViewModel addToResult = new TimetableDayViewModel();
                var listTimeTable = timetable.Where(x => x.DateTime == t)
                    .Select(t => new TimetableViewModel
                    {
                        TimeTableId = t.TimeTableId, 
                        Date = t.DateTime.ToString("dd/MM/yyyy"),
                        DayOfWeek = t.DateTime.ToString("dddd"),
                        SlotNumber = t.Slot.SlotName,
                        TimeStart = (TimeOnly)t.Slot.TimeStart,
                        TimeEnd = (TimeOnly)t.Slot.TimeEnd,
                        Room = t.Room.Name,
                        SubjectCode = t.Subject.SubjectCode,
                        SubjectName = t.Subject.SubjectName,
                        ClassCode = t.Class.ClassCode,
                        IsOnline = t.IsOnline ?? false
                    }).ToList();
                if (listTimeTable.Count > 0)
                {
                    addToResult = new TimetableDayViewModel
                    {
                        Date = t.ToString("dd/MM/yyyy"),
                        TimetableViewModels = listTimeTable,

                    };
                }
                else
                {
                    addToResult = new TimetableDayViewModel
                    {
                        Date = t.ToString("dd/MM/yyyy"),
                        TimetableViewModels = new List<TimetableViewModel>()
                    };
                }

                result.Add(addToResult);
            }

            return result;
        }

        public List<UserRollCallVM>? GetStudentInSlot(string timeTableId)
        {
            List<UserRollCallVM> result = new List<UserRollCallVM>();
            var timeTable = _context
                .TimeTables.Include(x => x.UserTimeTables)
                .ThenInclude(ut => ut.User)
                .FirstOrDefault(x => x.TimeTableId == timeTableId);
            foreach (var t in timeTable.UserTimeTables) 
            {
                result.Add(new UserRollCallVM 
                {
                    FullName = t.User.FullName,
                    StudentCode = t.User.StudentCode,
                    UserId = t.User.UserId,
                    RollCallStatus = t.RollCallStatus,
                });
            } 
            return result;         
        }

        public string RollCallStudent(List<RollCallVM> rollCallVM)
        {
            try
            {
                if (rollCallVM == null) throw new Exception("TimetableId is null");
                foreach(var t in rollCallVM)
                {
                    var record = _context.UserTimeTables
                        .FirstOrDefault(x => x.TimeTableId == t.TimeTableId && x.UserId == t.UserId);

                    if (record != null)
                    {
                        record.RollCallStatus = t.RollCallStatus;
                    }
                }
                _context.SaveChanges();
                var pending = _context.UserTimeTables
                    .Where(t => t.TimeTableId == rollCallVM[0].TimeTableId && t.RollCallStatus == 21)
                    .ToList();
                foreach (var item in pending)
                {
                    item.RollCallStatus = 20;
                }

                _context.SaveChanges();
                return "success";
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }

        public List<AttendanceReportVM> GetAttendanceReportVM(string userId)
        {
            var result = new List<AttendanceReportVM>();    
            var timetableGroup = _context.UserTimeTables
                .Where(c => c.UserId == userId)
                .GroupBy(t => t.SubjectId)
                .ToList();
            foreach(var g in timetableGroup)
            {
                var groupId = timetableGroup;
                var timeTableList = g.ToList();
                AttendanceReportVM addToResult = new AttendanceReportVM();
                addToResult.SubjectId = g.FirstOrDefault().SubjectId;
                addToResult.SubjectName = g.FirstOrDefault().SubjectName;
                addToResult.ClassName = g.FirstOrDefault().ClassName;
                addToResult.StartDate = g.OrderBy(x => x.Date).FirstOrDefault().Date.ToString();
                addToResult.EndDate = g.OrderByDescending(x => x.Date).FirstOrDefault().Date.ToString();
                addToResult.SlotPresent = g.Where(x => x.RollCallStatus == ConstantsValue.ROLL_CALL_PRESENT).Count();
                addToResult.SlotAbsent = g.Where(x => x.RollCallStatus == ConstantsValue.ROLL_CALL_ABSENT).Count();
                addToResult.AttendancePercent = (int)Math.Ceiling((double)addToResult.SlotPresent / (addToResult.SlotAbsent + addToResult.SlotPresent) * 100);
                result.Add(addToResult);
            }
            return result;
        }

        public AttendanceDetails GetAttendanceDetails(string userId, string subjectId)
        {
            var result = new AttendanceDetails();
            var timetableList = _context.UserTimeTables
                .Where(c => c.UserId == userId && c.SubjectId == subjectId)
                .OrderBy(c => c.Date)
                .ToList();
            if (timetableList.Count > 0)
            {
                var timetable = _context.TimeTables
                    .FirstOrDefault(t => t.SubjectId == subjectId && timetableList[0].TimeTableId == t.TimeTableId);

                if (timetable == null)
                {
                    throw new Exception("Timetable not found for the given subjectId and timetableList.");
                }

                var lecturer = _context.Users
                    .FirstOrDefault(x => x.UserId == timetable.UserId);

                if (lecturer == null)
                {
                    throw new Exception("Lecturer not found for the given timetable.");
                }

                var lecturerName = lecturer.UserName;
                result.SubjectName = timetableList[0].SubjectName ?? string.Empty;
                result.ClassName = timetableList[0].ClassName ?? string.Empty;
                result.SlotPresent = timetableList.Where(x => x.RollCallStatus == ConstantsValue.ROLL_CALL_PRESENT).Count();
                result.SlotAbsent = timetableList.Where(x => x.RollCallStatus == ConstantsValue.ROLL_CALL_ABSENT).Count();
                result.SlotNotyet = timetableList.Where(x => x.RollCallStatus == ConstantsValue.ROLL_CALL_NOT_YET).Count();
                result.AttendancePercent = (int)Math.Ceiling((double)result.SlotPresent / (result.SlotAbsent + result.SlotPresent) * 100);

                foreach (var i in timetableList)
                {
                    AttendanceItem addToListItem = new AttendanceItem
                    {
                        Date = i.Date?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Slot = _context.Slots.FirstOrDefault(x => x.SlotId == _context.TimeTables.FirstOrDefault(x => x.TimeTableId == i.TimeTableId).SlotId).SlotName ,
                        Lecturer = lecturerName ?? string.Empty,
                        RollCallStatus = i.RollCallStatus ?? ConstantsValue.ROLL_CALL_NOT_YET,
                    };
                    result.AttendanceItems.Add(addToListItem);
                }
            }
            return result;

        }

        public List<ClassUser> GetListClassWithStudent()
        {
            return _context.Classes
                .Include(c => c.Users)
                .Select(c => new ClassUser
                {
                    ClassId = c.ClassId,
                    ClassCode = c.ClassCode,
                    Users = c.Users.Select(u => new User
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        StudentCode = u.StudentCode
                    }).ToList()
                }).ToList();
        }

        public List<User> GetListTeacher()
        {
            return _context.Users
                .Where(u => u.Role == ConstantsValue.ROLE_TEACHER && u.IsDelete == false)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email,
                    StudentCode = u.StudentCode
                }).ToList();
        }
    }
}