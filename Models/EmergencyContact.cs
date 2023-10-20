using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class EmergencyContact
{
    public int EcId { get; set; }

    public int? EcHId { get; set; }

    public string? EcNumber1 { get; set; }

    public string? EcNumber2 { get; set; }

    public virtual Hospital? EcH { get; set; }
}
