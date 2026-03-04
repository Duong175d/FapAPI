using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class Notification
{
    public string NotificationId { get; set; } = null!;

    public DateTime? Date { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }
}
