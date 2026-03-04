using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class Subject
{
    public string SubjectId { get; set; } = null!;

    public string? SubjectCode { get; set; }

    public string? SubjectName { get; set; }

    public int? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? NumberSlot { get; set; }

    public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();

    public virtual ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();

    public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
