using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class Realtimewaitingtb
{
    public string? PEmail { get; set; }

    public DateTime? PDate { get; set; }

    public string? Status { get; set; }

    public int Sid { get; set; }

    public TimeSpan? PTime { get; set; }

    public string? PNumber { get; set; }
}
