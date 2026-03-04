using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class Room
{
    public string RoomId { get; set; } = null!;

    public string? Name { get; set; }

    public string Deparment { get; set; } = null!;

    public string? Floor { get; set; }

    public string? Number { get; set; }

    public int? IsDelete { get; set; }

    public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();

    public virtual ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();
}
