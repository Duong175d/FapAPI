using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class Letter
{
    public string LetterId { get; set; } = null!;

    public string? UserId { get; set; }

    public int? Type { get; set; }

    public string? Reason { get; set; }

    public string? CodeSubject { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }

    public bool? IsDelete { get; set; }

    public string? Response { get; set; }

    public virtual User? User { get; set; }
}
