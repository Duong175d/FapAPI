using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class Class
{
    public string ClassId { get; set; } = null!;

    public string ClassCode { get; set; } = null!;

    public int IsDelete { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual ICollection<TimeTable> TimeTables { get; set; } = new List<TimeTable>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
