using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class Appointment
{
    public int AId { get; set; }

    public int? AHId { get; set; }

    public int? ADId { get; set; }

    public string? APatientName { get; set; }

    public DateTime? APatientDob { get; set; }

    public DateTime? ADate { get; set; }

    public TimeSpan? ATime { get; set; }

    public string? AMobile { get; set; }

    public string? AEmail { get; set; }

    public string? AType { get; set; }

    public string? AReason { get; set; }

    public virtual Doctor? AD { get; set; }

    public virtual Hospital? AH { get; set; }
}
