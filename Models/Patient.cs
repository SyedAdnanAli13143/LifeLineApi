using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class Patient
{
    public int PId { get; set; }

    public int? PDId { get; set; }

    public string? PName { get; set; }

    public DateTime? PDob { get; set; }

    public string? PMobile { get; set; }

    public DateTime? PDate { get; set; }

    public TimeSpan? PTime { get; set; }

    public string? PAStatus { get; set; }

    public string? PReason { get; set; }

    public string? PEmail { get; set; }

    public string? PPassword { get; set; }

    public virtual ICollection<DoctorPrescription> DoctorPrescriptions { get; set; } = new List<DoctorPrescription>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<MedicalPortfolio> MedicalPortfolios { get; set; } = new List<MedicalPortfolio>();

    public virtual Doctor? PD { get; set; }

    public virtual ICollection<VideoConsultation> VideoConsultations { get; set; } = new List<VideoConsultation>();
}
