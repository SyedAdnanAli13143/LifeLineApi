using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class VideoConsultation
{
    public int VcId { get; set; }

    public int? VcPId { get; set; }

    public int? VcDId { get; set; }

    public DateTime? VcDate { get; set; }

    public TimeSpan? VcTime { get; set; }

    public virtual Doctor? VcD { get; set; }

    public virtual Patient? VcP { get; set; }
}
