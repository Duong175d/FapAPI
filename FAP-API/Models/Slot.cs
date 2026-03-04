using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class Slot
{
    public string SlotId { get; set; } = null!;

    public string? SlotName { get; set; }

    public TimeOnly? TimeStart { get; set; }

    public TimeOnly? TimeEnd { get; set; }

    public int? SlotNumber { get; set; }

    public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();

    public virtual ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();
}
