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

    public virtual Doctor? DpD { get; set; }

    public virtual Patient? DpP { get; set; }

    public virtual ICollection<MedicalPortfolio> MedicalPortfolios { get; set; } = new List<MedicalPortfolio>();

    public virtual ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = new List<PrescriptionMedication>();
}
