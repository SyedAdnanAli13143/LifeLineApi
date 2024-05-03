using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class DoctorPrescription
{
    public int DpId { get; set; }

    public int? DpDId { get; set; }

    public int? DpPId { get; set; }

    public DateTime? DpDate { get; set; }

    public string? DpDisease { get; set; }

    public string? DpMedicine { get; set; }

    public TimeSpan? DpScheduleTime { get; set; }

    public DateTime? DpStartDate { get; set; }

    public DateTime? DpEndDate { get; set; }

    public DateTime? SentDate { get; set; }

    public virtual Doctor? DpD { get; set; }

    public virtual Patient? DpP { get; set; }
}
