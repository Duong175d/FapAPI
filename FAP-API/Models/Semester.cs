using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class Semester
{
    public string SemesterId { get; set; } = null!;

    public string? SemesterName { get; set; }

    public virtual ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
