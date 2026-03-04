namespace FAP_API.Constants
{
    public static class ConstantsValue
    {
        //Quyền tài khoản
        public const int ROLE_ADMIN = 2;
        public const int ROLE_TEACHER = 3;
        public const int ROLE_STUDENT = 4;
        public const int ROLE_TESTING = 5;

        // Loại bài kiểm tra (EXAM_TYPE)
        public const int EXAM_TYPE_FE = 7;
        public const int EXAM_TYPE_2NDFE = 8;
        public const int EXAM_TYPE_PE = 9;

        // Ghi chú lịch kiểm tra (EXAM_NOTE)
        public const int EXAM_NOTE_MULTI_CHOICES_EOS = 11;
        public const int EXAM_NOTE_PRACTICAL_EXAM_EOS = 12;
        public const int EXAM_NOTE_PRACTICAL_EXAM_PEA = 13;
        public const int EXAM_NOTE_READING_WRITING_EOS = 14;
        public const int EXAM_NOTE_PROJECT_PRESENT = 15;
        public const int EXAM_NOTE_RVW_ON_EOS = 16;
        public const int EXAM_NOTE_THUC_HANH = 17;

        // Trạng thái điểm danh (ROLL_CALL_STATUS)
        public const int ROLL_CALL_STATUS = 18;
        public const int ROLL_CALL_PRESENT = 19;
        public const int ROLL_CALL_ABSENT = 20;
        public const int ROLL_CALL_NOT_YET = 21;

        // Học kì (SEMESTER)
        public const int SEMESTER_SPRING = 23;
        public const int SEMESTER_SUMMER = 24;
        public const int SEMESTER_FALL = 25;

        // Trạng thái nộp đơn (STATUS_LATTER)
        public const int STATUS_LATTER_IN_PROCESS = 27;
        public const int STATUS_LATTER_REJECTED = 28;
        public const int STATUS_LATTER_APPROVED = 29;
        public const int STATUS_LATTER_UNREAD = 30;


        // Loại đơn (TYPE_LETTER)
        public const int TYPE_LETTER_ATTENDANCE_EXEMPTION = 32;
        public const int TYPE_LETTER_ACADEMIC_TRANSCRIPT = 33;
        public const int TYPE_LETTER_RE_EVALUATION = 34;
        public const int TYPE_LETTER_RETAKE = 35;
        public const int TYPE_LETTER_OTHER = 36;
    }

    public static class ConstantsDescription
    {
        // Quyền tài khoản
        public const string ROLE_ADMIN = "ROLE_ADMIN";
        public const string ROLE_TEACHER = "ROLE_TEACHER";
        public const string ROLE_STUDENT = "ROLE_STUDENT";
        public const string ROLE_TESTING = "ROLE_TESTING";

        // Loại bài kiểm tra (EXAM_TYPE)
        public const string EXAM_TYPE_FE = "Thi FE";
        public const string EXAM_TYPE_2NDFE = "Thi lại FE";
        public const string EXAM_TYPE_PE = "Thi PE";

        // Ghi chú lịch kiểm tra (EXAM_NOTE)
        public const string EXAM_NOTE_MULTI_CHOICES_EOS = "Multiple_choices on EOS";
        public const string EXAM_NOTE_PRACTICAL_EXAM_EOS = "Practical_Exam (EOS client)";
        public const string EXAM_NOTE_PRACTICAL_EXAM_PEA = "Practical_Exam (PEA client)";
        public const string EXAM_NOTE_READING_WRITING_EOS = "Reading, Writing on EOS";
        public const string EXAM_NOTE_PROJECT_PRESENT = "Project Presentation";
        public const string EXAM_NOTE_RVW_ON_EOS = "Reading, Vocabulary, Writing on EOS";
        public const string EXAM_NOTE_THUC_HANH = "Thực hành";

        // Trạng thái điểm danh (ROLL_CALL_STATUS)
        public const string ROLL_CALL_PRESENT = "present";
        public const string ROLL_CALL_ABSENT = "absent";
        public const string ROLL_CALL_NOT_YET = "not yet";

        // Học kì (SEMESTER)
        public const string SEMESTER_SPRING = "Spring";
        public const string SEMESTER_SUMMER = "Summer";
        public const string SEMESTER_FALL = "Fall";

        // Trạng thái nộp đơn (STATUS_LATTER)
        public const string STATUS_LATTER_IN_PROCESS = "Đang chờ xử lý";
        public const string STATUS_LATTER_REJECTED = "Reject";
        public const string STATUS_LATTER_APPROVED = "Approved";
        public const string STATUS_LATTER_UNREAD = "Chưa đọc";

        // Loại đơn (TYPE_LETTER)
        public const string TYPE_LETTER_ATTENDANCE_EXEMPTION = "Đề nghị miễn điểm danh";
        public const string TYPE_LETTER_ACADEMIC_TRANSCRIPT = "Cấp bảng điểm quá trình";
        public const string TYPE_LETTER_RE_EVALUATION = "Đơn xin phúc khảo";
        public const string TYPE_LETTER_RETAKE = "Thi cải thiện điểm";
        public const string TYPE_LETTER_OTHER = "Những loại đơn khác";
    }

    public static class UserRoleEnum
    {
        public enum UserRoleE
        {
            ROLE_ADMIN = 2,
            ROLE_TEACHER = 3,
            ROLE_STUDENT = 4,
            ROLE_TESTING = 5,
        }
    }

    public static class RoleString
    {
        public const string Admin = "2";
        public const string Teacher = "3";
        public const string Student = "4"; 
    }
}