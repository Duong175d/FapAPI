using System.Xml;
using FAP_API.Constants;
using FAP_API.Models;
using FAP_API.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FAP_API.Services
{
    public interface IExamScheduleService
    {
        void CreateExamSchedule(ExamScheduleViewModel examScheduleViewModel);
        public List<ExamUserViewModel> GetExam(string userId, string semesterName);
        void UpdateExamSchedule(ExamScheduleUpdateViewModel examScheduleViewModel);
        void DeleteExamSchedule(ExamSchedule examSchedule);
        public List<object> getUserById(string examId);
        List<ExamSchedule> getExambySemester(int semester);
        public List<string> GetStudentSemesters(string userId);
        public UserTimeTable GetMaxDateValid(string subjectId);
        public List<Room> GetRoomByDate(DateOnly date);
        public List<Room> GetRoomBySlot(int slot);
        public bool IsRoomAvailable(string roomName, DateOnly examDate, string slotTime);
        public List<ExamSchedulerVM> GetExamAmin(string semesterName);
        public List<RoomDto> GetAllRooms();
        public List<SubjectDto> GetAllSubjects();
        public ExamScheduleUpdateViewModel GetExamById(string esId);

        public List<string> GetSemesterLecture();

        public List<LecturerDto> GetAllLecturer();

    }
    public class ExamScheduleService : IExamScheduleService
    {
        private readonly Prm392Context _context;

        public ExamScheduleService(Prm392Context prm392Context)
        {
            _context = prm392Context;
        }
        public void CreateExamSchedule(ExamScheduleViewModel examScheduleViewModel)
        {
            if (examScheduleViewModel.RoomName == null || examScheduleViewModel.SlotTime == null ||
                   examScheduleViewModel.Note == null || examScheduleViewModel?.Type == null || examScheduleViewModel.ExamDate == null
               ) throw new ArgumentException("Invalid Data");

            var today = DateOnly.FromDateTime(DateTime.Today);

            var checkDate = GetMaxDateValid(examScheduleViewModel.SubjectCode);

            if (examScheduleViewModel.ExamDate > checkDate.Date)
            {
                if (examScheduleViewModel.ExamDate > today)
                {
                    var isAvailable = IsRoomAvailable(
                   examScheduleViewModel.RoomName,
                   examScheduleViewModel.ExamDate.Value,
                   examScheduleViewModel.SlotTime
               );
                    var subjectId = GetSubjectIdByCode(examScheduleViewModel.SubjectCode);
                    var slotId = GetSlotIdByName(examScheduleViewModel.SlotTime);
                    var roomId = GetRoomIdByName(examScheduleViewModel.RoomName);

                    var eligibleStudents = GetStudentsForExam(subjectId, examScheduleViewModel.ExamDate.Value, slotId, roomId);

                    if (!eligibleStudents.Any())
                        throw new Exception("Không có sinh viên hợp lệ để tạo bài thi");

                    if (!isAvailable)
                        throw new Exception("The room is already booked for this date and slot");

                    var getSemes = GetSemesterName(checkDate.Date.Value);

                    var exam = new ExamSchedule
                    {
                        EsId = Guid.NewGuid().ToString(),
                        RoomId = roomId,
                        SubjectId = subjectId,
                        SlotId = slotId,
                        ExamDate = examScheduleViewModel.ExamDate.Value,
                        Semester = getSemes,
                        Note = examScheduleViewModel.Note,
                        Type = examScheduleViewModel.Type
                    };


                    _context.ExamSchedules.Add(exam);
                    _context.SaveChanges();
                    var examUsers = eligibleStudents.Select(student => new ExamUser
                    {
                        EsId = exam.EsId,
                        UserId = student.UserId,
                        IsTeacher = student.Role == ConstantsValue.ROLE_STUDENT ? 0 : 1,
                    }).ToList();

                    _context.ExamUsers.AddRange(examUsers);
                    _context.SaveChanges();

                    // tạo retake
                    if (examScheduleViewModel.Type == ConstantsValue.EXAM_TYPE_FE)
                    {
                        var exam2 = new ExamSchedule
                        {
                            EsId = Guid.NewGuid().ToString(),
                            RoomId = "00000000-0000-0000-0000-000000000000",
                            SubjectId = subjectId,
                            SlotId = slotId,
                            ExamDate = examScheduleViewModel.ExamDate.Value.AddDays(7),
                            Semester = getSemes,
                            Note = examScheduleViewModel.Note,
                            Type = ConstantsValue.EXAM_TYPE_2NDFE
                        };


                        _context.ExamSchedules.Add(exam2);
                        _context.SaveChanges();

                        var examUsers2 = eligibleStudents.Select(student => new ExamUser
                        {
                            EsId = exam2.EsId,
                            UserId = student.UserId,
                            IsTeacher = student.Role == ConstantsValue.ROLE_STUDENT ? 0 : 1
                        }).ToList();

                        _context.ExamUsers.AddRange(examUsers2);
                        _context.SaveChanges();
                    }
                }
            }
            else
            {
                throw new Exception("Exam date is not valid");
            }

        }
        public void DeleteExamSchedule(ExamSchedule examSchedule)
        {
            var existing = _context.ExamSchedules.FirstOrDefault(t => t.EsId == examSchedule.EsId);

            if (existing == null)
            {
                throw new Exception("not found.");
            }

            existing.IsDelete = true;
            _context.SaveChanges();
        }

        public List<ExamUserViewModel> GetExam(string userId, string semesterName)
        {
            var exams = _context.ExamUsers
              .Include(e => e.User)
              .Include(e => e.Es)
                .ThenInclude(es => es.Subject)
              .Include(e => e.Es)
                .ThenInclude(es => es.Room)
              .Include(e => e.Es)
                .ThenInclude(es => es.Slot)
              .Where(e => e.UserId == userId)
              .Select(e => new ExamUserViewModel
              {
                  UserId = e.UserId,
                  FullName = e.User.FullName,
                  ExamId = e.EsId,
                  SubjectCode = e.Es.Subject.SubjectCode,
                  RoomName = e.Es.Room.Name,
                  SlotTime = e.Es.Slot.SlotName,
                  ExamDate = e.Es.ExamDate,
                  Semester = e.Es.Semester,
                  TimeStart = e.Es.Slot.TimeStart,
                  TimeEnd = e.Es.Slot.TimeEnd,
                  Note = e.Es.Note,
                  SemesterName = e.Es.ExamDate.HasValue
                  ? GetSemesterName(e.Es.ExamDate.Value)
                  : "unknown"
              })
              .AsEnumerable()
              .Where(x => x.SemesterName.ToUpper() == semesterName.ToUpper())
              .OrderBy(x => x.ExamDate)
              .ToList();

            return exams;
        }

        public List<ExamSchedulerVM> GetExamAmin(string semesterName)
        {
            var examsRaw = _context.ExamSchedules
                 .Include(es => es.ExamUsers)
                     .ThenInclude(e => e.User)
                 .Include(es => es.Subject)
                 .Include(es => es.Room)
                 .Include(es => es.Slot)
                 .ToList();

            var exams = examsRaw.Select(e => new ExamSchedulerVM
            {
                EsId = e.EsId,
                SubjectName = e.Subject.SubjectName,
                SubjectCode = e.Subject.SubjectCode,
                RoomName = e.Room.Name,
                SlotName = e.Slot.SlotName,
                SlotNumber = e.Slot.SlotNumber,
                UserName = e.ExamUsers.Where(es => es.User.Role == ConstantsValue.ROLE_TEACHER)
                            .Select(es => es.User.UserName)
                            .FirstOrDefault(),
                ExamDate = e.ExamDate,
                TimeStart = e.Slot.TimeStart,
                TimeEnd = e.Slot.TimeEnd,
                Note = e.Note,
                Type = e.Type,
                SemesterName = e.ExamDate.HasValue
                  ? GetSemesterName(e.ExamDate.Value)
                  : "unknown"
            }).AsEnumerable()
              .Where(x => x.SemesterName.ToUpper() == semesterName.ToUpper())
              .OrderBy(x => x.ExamDate)
              .ToList();
            return exams;
        }
        public static string GetSemesterName(DateOnly examDate)
        {
            string semName = examDate.Month switch
            {
                >= 1 and <= 4 => ConstantsDescription.SEMESTER_SPRING,
                >= 5 and <= 8 => ConstantsDescription.SEMESTER_SUMMER,
                >= 9 and <= 12 => ConstantsDescription.SEMESTER_FALL,
                _ => "unknown"
            };

            return $"{semName}{examDate.Year % 100}";
        }


        public List<ExamSchedule> getExambySemester(int semester)
        {
            var ltexam = _context.ExamSchedules
                .Where(eu => eu.Semester.Equals(semester))
                .ToList();

            if (!ltexam.Any())
            {
                throw new Exception("No users found for the given exam ID.");
            }

            return ltexam;
        }

        public List<object> getUserById(string examId)
        {
            var users = _context.ExamUsers
                .Where(eu => eu.EsId == examId)
                .Include(eu => eu.User)
                .Select(eu => eu.User)
                .ToList();

            if (!users.Any())
            {
                throw new Exception("No users found for the given exam ID.");
            }

            var orderedUsers = users
                .OrderBy(u => u.Role == 3 ? 0 : 1)
                .ToList();

            var result = orderedUsers.Select((u, index) => new
            {
                index = index + 1,
                email = u.Email,
                role = u.Role == 3 ? "Lecturer" : (u.Role == 4 ? "Student" : "Unknown"),
                fullName = u.FullName,
                userName = u.UserName,
                status = u.Status,
                createDate = u.CreateDate,
                updateDate = u.UpdateDate,
            }).ToList<object>();

            return result;
        }

        public List<string> GetStudentSemesters(string userId)
        {
            var rawSemesters = _context.ExamSchedules
                .Include(e => e.ExamUsers)
                .Where(e => e.ExamUsers.Any(u => u.UserId == userId))
                .Select(e => e.ExamDate)
                .Distinct()
                .ToList();

            var semesterList = rawSemesters
                .Where(date => date.HasValue)
                .Select(date =>
                {
                    var d = date.Value;
                    string semName = d.Month switch
                    {
                        >= 1 and <= 4 => "SPRING",
                        >= 5 and <= 8 => "SUMMER",
                        >= 9 and <= 12 => "FALL",
                        _ => "UNKNOWN"
                    };
                    return $"{semName}{(d.Year % 100)}";
                })
                .Distinct()
                .ToList();

            var seasonsPriority = new Dictionary<string, int>
    {
        { "FALL", 0 },
        { "SUMMER", 1 },
        { "SPRING", 2 }
    };

            var orderedSemesterList = semesterList
                .OrderBy(s =>
                {
                    var match = System.Text.RegularExpressions.Regex.Match(s, @"^(FALL|SUMMER|SPRING)(\d{2})$");
                    if (!match.Success)
                        return int.MaxValue;

                    var season = match.Groups[1].Value;
                    var year = int.Parse(match.Groups[2].Value);

                    return (100 - year) * 10 + seasonsPriority[season];
                })
                .ToList();

            return orderedSemesterList;
        }


        public void UpdateExamSchedule(ExamScheduleUpdateViewModel examScheduleViewModel)
        {
            if (string.IsNullOrEmpty(examScheduleViewModel.EsId) ||
                string.IsNullOrEmpty(examScheduleViewModel.RoomName) ||
                string.IsNullOrEmpty(examScheduleViewModel.SlotTime) ||
                string.IsNullOrEmpty(examScheduleViewModel.SubjectCode) ||
                examScheduleViewModel.ExamDate == null ||
                examScheduleViewModel.Note == null)
            {
                throw new ArgumentException("Invalid Data");
            }

            var existing = _context.ExamSchedules.FirstOrDefault(t => t.EsId == examScheduleViewModel.EsId);
            if (existing == null)
                throw new Exception("Exam schedule not found");

            var today = DateOnly.FromDateTime(DateTime.Today);
            var checkDate = GetMaxDateValid(examScheduleViewModel.SubjectCode);

            //if (examScheduleViewModel.ExamDate < today || checkDate.DateTime < examScheduleViewModel.ExamDate)
            //    throw new Exception("Invalid date");


            if (examScheduleViewModel.ExamDate > checkDate.Date)
            {
                if (examScheduleViewModel.ExamDate > today)
                {
                    var isAvailable = IsRoomAvailable(
                examScheduleViewModel.RoomName,
                examScheduleViewModel.ExamDate.Value,
                examScheduleViewModel.SlotTime,
                existing.EsId // tránh so sánh chính mình nếu update giữ nguyên slot cũ
            );



                    if (!isAvailable)
                        throw new Exception("The room is already booked for this date and slot");




                    var getSemes = GetSemesterName(checkDate.Date.Value);


                    existing.SubjectId = GetSubjectIdByCode(examScheduleViewModel.SubjectCode);
                    existing.SlotId = GetSlotIdByName(examScheduleViewModel.SlotTime);
                    existing.RoomId = GetRoomIdByName(examScheduleViewModel.RoomName);
                    if (examScheduleViewModel.Type == ConstantsValue.EXAM_TYPE_2NDFE)
                    {
                        var date2Nd = CheckDate2ND(examScheduleViewModel.EsId);
                        if (date2Nd.ExamDate < examScheduleViewModel.ExamDate)
                        {
                            existing.ExamDate = examScheduleViewModel.ExamDate.Value;
                        }
                        else
                        {
                            throw new Exception("ngay thi lai phai lon hon ngay thi dau tien");
                        }
                    }
                    existing.ExamDate = examScheduleViewModel.ExamDate.Value;
                    existing.Semester = getSemes;
                    existing.Note = examScheduleViewModel.Note;
                    existing.Type = examScheduleViewModel.Type;

                    RemoveLecturer(existing.EsId);
                    AddLecturer(examScheduleViewModel.UserName, existing.EsId);
                    _context.SaveChanges();


                }
            }
            else
            {
                throw new Exception("Exam date is not valid");
            }
        }

        private ExamSchedule CheckDate2ND(string esId)
        {
            var query1 = _context.ExamSchedules.Where(e => e.EsId == esId).FirstOrDefault();

            if (query1 != null)
            {
                var query2 = _context.ExamSchedules.Where(ex => ex.Semester == query1.Semester && ex.SubjectId == query1.SubjectId
                && ex.Type == ConstantsValue.EXAM_TYPE_FE).FirstOrDefault();

                return query2;
            }
            else
                throw new Exception();

        }


        private void RemoveLecturer(string esId)
        {
            var query = _context.ExamUsers
                .Include(eu => eu.Es)
                .Where(eu => eu.EsId == esId && eu.IsTeacher == 1).FirstOrDefault();

            _context.ExamUsers.Remove(query);
            _context.SaveChanges();
        }

        private void AddLecturer(string userName, string esId)
        {
            var query = _context.Users
               .Where(u => u.Role == ConstantsValue.ROLE_TEACHER && u.UserName == userName).FirstOrDefault();

            if (query == null)
            {
                throw new Exception("User not found: " + userName);
            }

            var lecturer = new ExamUser
            {
                EsId = esId,
                UserId = query.UserId,
                IsTeacher = 1
            };

            _context.ExamUsers.Add(lecturer);
            _context.SaveChanges();
        }
        public bool IsRoomAvailable(string roomName, DateOnly examDate, string slotTime, string excludeExamId = null)
        {
            var roomId = GetRoomIdByName(roomName);
            var slotId = GetSlotIdByName(slotTime);

            return !_context.ExamSchedules.Any(e =>
                e.RoomId == roomId &&
                e.SlotId == slotId &&
                e.ExamDate == examDate &&
                (excludeExamId == null || e.EsId != excludeExamId));
        }

        private Subject getSubjectId(string subjectCode)
        {
            var subject = _context.Subjects.Where(s => s.SubjectCode == subjectCode).FirstOrDefault();
            if(subject == null)
            {
                throw new Exception("SubjectId not found");
            }
            return subject;
        }

        public UserTimeTable GetMaxDateValid(string subjectId)
        {
            string subjectID = getSubjectId(subjectId).SubjectId;

            var findMaxDate = _context.UserTimeTables
                .Where(e => e.SubjectId == subjectID)
                .OrderByDescending(t => t.Date)
                .FirstOrDefault();

            return findMaxDate;
        }

        public List<Room> GetRoomByDate(DateOnly date)
        {
            var list = new List<Room>();
            return list;

        }

        public List<Room> GetRoomBySlot(int slot)
        {
            throw new NotImplementedException();
        }

        public bool IsRoomAvailable(string roomName, DateOnly examDate, string slotTime)
        {
            return !_context.ExamSchedules.Any(e =>
                 e.Room.Name == roomName &&
                 e.ExamDate == examDate &&
                 e.Slot.SlotName == slotTime
    );
        }


        public string GetRoomIdByName(string roomName)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Name == roomName);
            return room.RoomId;
        }

        public string GetSlotIdByName(string slotName)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.SlotName == slotName);
            return slot.SlotId;
        }

        public string GetSubjectIdByCode(string subjectCode)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.SubjectCode == subjectCode);
            return subject.SubjectId;
        }
        public List<User> GetStudentsForExam(string subjectId, DateOnly examDate, string slotId, string roomId)
        {
            string examTermCode = GetSemesterName(examDate);

            var currentStudents = _context.UserTimeTables
                   .Include(tt => tt.User)
                   .Where(tt => tt.SubjectId == subjectId  && tt.User.Role == ConstantsValue.ROLE_STUDENT)
                   .AsEnumerable()
                   .Where(tt => GetSemesterName(tt.Date.Value) == examTermCode)
                   .Select(tt => tt.User)
                   .Distinct()
                   .ToList();

            var eligibleStudents = new List<User>();

            foreach (var student in currentStudents)
            {
                if (eligibleStudents.Count >= 15)
                    break;

                var userId = student.UserId;

                var examAttempts = _context.ExamUsers
                    .Include(eu => eu.Es)
                    .Where(eu =>
                        eu.UserId == userId &&
                        eu.Es.SubjectId == subjectId)
                    .AsEnumerable()
                    .Where(eu => GetSemesterName(eu.Es.ExamDate.Value) == examTermCode)
                    .ToList();

                bool hasSlotConflict = examAttempts.Any(ea =>
                    ea.Es.ExamDate == examDate && ea.Es.SlotId == slotId && ea.Es.RoomId == roomId);

                if (hasSlotConflict)
                    continue;

                eligibleStudents.Add(student);
            }

            // Lấy danh sách tất cả giảng viên
            var allLecturer = _context.Users
                .Where(u => u.Role == ConstantsValue.ROLE_TEACHER)
                .ToList();
            var random = new Random();
            var shuffledLecturers = allLecturer.OrderBy(l => random.Next()).ToList();

            var eligibleLecturer = shuffledLecturers.FirstOrDefault(lecture =>
            {
                var examAttempts = _context.ExamUsers
                    .Include(eu => eu.Es)
                    .Where(eu =>
                        eu.UserId == lecture.UserId &&
                        eu.IsTeacher == 1)
                    .ToList();

                return !examAttempts.Any(ea =>
                    ea.Es.ExamDate == examDate &&
                    ea.Es.SlotId == slotId &&
                    ea.Es.RoomId == roomId);
            });

            if (eligibleLecturer != null)
            {
                eligibleStudents.Add(eligibleLecturer);
            }

            return eligibleStudents.OrderBy(u => u.FullName).ToList();
        }

        public List<RoomDto> GetAllRooms()
        {
            return _context.Rooms
                .Select(x => new RoomDto
                {
                    RoomId = x.RoomId,
                    Name = x.Name
                })
                .ToList();
        }


        public List<SubjectDto> GetAllSubjects()
        {
            return _context.Subjects
                .Select(x => new SubjectDto
                {
                    SubjectId = x.SubjectId,
                    SubjecCode = x.SubjectCode
                })
                .ToList();
        }

        public List<string> GetSemesterLecture()
        {
            var seasonsPriority = new Dictionary<string, int>
            {
                { "FALL", 0 },
                { "SUMMER", 1 },
                { "SPRING", 2 }
            };

            return _context.ExamSchedules
                .Select(x => x.Semester.ToUpper())
                .Distinct()
                .AsEnumerable()
                .OrderBy(s =>
                {
                    var match = System.Text.RegularExpressions.Regex.Match(s, @"^(FALL|SUMMER|SPRING)(\d{2})$");
                    if (!match.Success)
                        return int.MaxValue;

                    var season = match.Groups[1].Value;
                    var year = int.Parse(match.Groups[2].Value);
                    return (100 - year) * 10 + seasonsPriority[season];
                })
                .ToList();
        }

        public ExamScheduleUpdateViewModel GetExamById(string esId)
        {
            var query = _context.ExamSchedules
                .Include(es => es.ExamUsers)
                .Include(es => es.Room)
                .Include(es => es.Slot)
                .Include(es => es.Subject)
                .Where(es => es.EsId == esId)
                .Select(e => new ExamScheduleUpdateViewModel
                {
                    EsId = e.EsId,
                    UserName = e.ExamUsers.FirstOrDefault() != null ? e.ExamUsers.FirstOrDefault().User.UserName : null,
                    SubjectCode = e.Subject.SubjectCode,
                    RoomName = e.Room.Name,
                    SlotTime = e.Slot.SlotName,
                    ExamDate = e.ExamDate,
                    Note = e.Note,
                    Type = e.Type.Value
                }).FirstOrDefault();


            if (query != null)
            {
                return query;
            }

            throw new Exception();
        }

        public List<LecturerDto> GetAllLecturer()
        {
            return _context.Users
                 .Where(e => e.Role == ConstantsValue.ROLE_TEACHER)
                .Select(e => new LecturerDto
                {
                    UserId = e.UserId,
                    UserName = e.UserName
                })

                .ToList();
        }
    }
}