using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class Doctor
{
    public int DId { get; set; }

    public int? DHId { get; set; }

    public string? DName { get; set; }

    public string? DEmail { get; set; }

    public string? DPassword { get; set; }

    public string? DMobile { get; set; }

    public string? DField { get; set; }

    public string? DAvailablityStatus { get; set; } 


    
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    
    public virtual Hospital? DH { get; set; }
    
    public virtual ICollection<DoctorPrescription> DoctorPrescriptions { get; set; } = new List<DoctorPrescription>();
    
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
   
    public virtual ICollection<VideoConsultation> VideoConsultations { get; set; } = new List<VideoConsultation>();
}
