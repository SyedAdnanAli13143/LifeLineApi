using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class Feedback
{
    public int FId { get; set; }

    public int? FDId { get; set; }

    public int? FPId { get; set; }

    public int? FRating { get; set; }

    public string? FComments { get; set; }

    public virtual Doctor? FD { get; set; }

    public virtual Patient? FP { get; set; }
}
