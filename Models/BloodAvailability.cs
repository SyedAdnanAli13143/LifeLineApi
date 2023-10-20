using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class BloodAvailability
{
    public int BaId { get; set; }

    public int? BaHId { get; set; }

    public string? BaBloodGroup { get; set; }

    public int? BaBottlesAvailable { get; set; }

    public DateTime? BaDate { get; set; }

    public TimeSpan? BaTime { get; set; }

    public virtual Hospital? BaH { get; set; }
}
