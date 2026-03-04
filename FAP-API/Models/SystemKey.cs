using System;
using System.Collections.Generic;

namespace FAP_API.Models;

public partial class SystemKey
{
    public int Id { get; set; }

    public int? ParrentId { get; set; }

    public string? CodeKey { get; set; }

    public string? Description { get; set; }

    public int? SortOrder { get; set; }
}
