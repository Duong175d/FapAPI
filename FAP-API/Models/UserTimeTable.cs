using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class UserTimeTable
{
    public string UserId { get; set; } = null!;

    public string TimeTableId { get; set; } = null!;

    /// <summary>
    /// 19-P, 20-A, 21-N
    /// </summary>
    public int? RollCallStatus { get; set; }

    public string? SubjectId { get; set; }

    public DateOnly? Date { get; set; }

    public string? ClassName { get; set; }

    public string? SubjectName { get; set; }

    public virtual TimeTable TimeTable { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
