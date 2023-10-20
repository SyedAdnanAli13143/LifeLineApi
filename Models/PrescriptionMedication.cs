using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class PrescriptionMedication
{
    public int PmId { get; set; }

    public int? PmDpId { get; set; }

    public string? PmMedicine { get; set; }

    public string? PmDosage { get; set; }

    public TimeSpan? PmScheduleTime { get; set; }

    public DateTime? PmStartDate { get; set; }

    public DateTime? PmEndDate { get; set; }

    public virtual ICollection<MedicalPortfolio> MedicalPortfolios { get; set; } = new List<MedicalPortfolio>();

    public virtual DoctorPrescription? PmDp { get; set; }
}
