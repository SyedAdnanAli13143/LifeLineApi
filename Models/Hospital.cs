using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeLineApi.Models;

public partial class Hospital
{
    public int HId { get; set; }

    public string? HName { get; set; }

    public string? HAddress { get; set; }

    public string? HEmail { get; set; }

    public string? HPassword { get; set; }

    public double HlLatitude { get; set; }

    public double HlLongitude { get; set; }
    [NotMapped]
    public double? DistanceInKm { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<BloodAvailability> BloodAvailabilities { get; set; } = new List<BloodAvailability>();

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();

    public virtual ICollection<HEmployee> HEmployees { get; set; } = new List<HEmployee>();

    public virtual ICollection<HospitalService> HospitalServices { get; set; } = new List<HospitalService>();
}
