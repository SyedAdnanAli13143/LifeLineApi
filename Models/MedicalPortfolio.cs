using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class MedicalPortfolio
{
    public int MpId { get; set; }

    public int? MpPId { get; set; }

    public int? MpDpId { get; set; }

    public int? MpPmId { get; set; }

    public virtual DoctorPrescription? MpDp { get; set; }

    public virtual Patient? MpP { get; set; }

    public virtual PrescriptionMedication? MpPm { get; set; }
}
