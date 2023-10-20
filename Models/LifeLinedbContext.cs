using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LifeLineApi.Models;

public partial class LifeLinedbContext : DbContext
{
    public LifeLinedbContext()
    {
    }

    public LifeLinedbContext(DbContextOptions<LifeLinedbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationAdmin> ApplicationAdmins { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<BloodAvailability> BloodAvailabilities { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorPrescription> DoctorPrescriptions { get; set; }

    public virtual DbSet<EmergencyContact> EmergencyContacts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<HEmployee> HEmployees { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<MedicalPortfolio> MedicalPortfolios { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PrescriptionMedication> PrescriptionMedications { get; set; }

    public virtual DbSet<VideoConsultation> VideoConsultations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-646VG22\\SQLEXPRESS;Database=LifeLinedb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationAdmin>(entity =>
        {
            entity.HasKey(e => e.AaId).HasName("PK__Applicat__509C98E66D8C254A");

            entity.ToTable("Application_Admin");

            entity.Property(e => e.AaId).HasColumnName("AA_ID");
            entity.Property(e => e.AaEmail)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("AA_Email");
            entity.Property(e => e.AaPassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("AA_Password");
            entity.Property(e => e.AaUserName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("AA_UserName");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AId).HasName("PK__Appointm__71AC6D412B7481FC");

            entity.Property(e => e.AId).HasColumnName("A_ID");
            entity.Property(e => e.ADId).HasColumnName("A_D_ID");
            entity.Property(e => e.ADate)
                .HasColumnType("date")
                .HasColumnName("A_Date");
            entity.Property(e => e.AEmail)
                .HasMaxLength(50)
                .HasColumnName("A_Email");
            entity.Property(e => e.AHId).HasColumnName("A_H_ID");
            entity.Property(e => e.AMobile)
                .HasMaxLength(11)
                .HasColumnName("A_Mobile");
            entity.Property(e => e.APatientDob)
                .HasColumnType("date")
                .HasColumnName("A_PatientDOB");
            entity.Property(e => e.APatientName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("A_PatientName");
            entity.Property(e => e.AReason)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("A_Reason");
            entity.Property(e => e.ATime).HasColumnName("A_Time");
            entity.Property(e => e.AType)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("A_Type");

            entity.HasOne(d => d.AD).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ADId)
                .HasConstraintName("FK__Appointme__A_D_I__5165187F");

            entity.HasOne(d => d.AH).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AHId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Appointme__A_H_I__5070F446");
        });

        modelBuilder.Entity<BloodAvailability>(entity =>
        {
            entity.HasKey(e => e.BaId).HasName("PK__Blood_Av__6DFCF7B82E636F43");

            entity.ToTable("Blood_Availability");

            entity.Property(e => e.BaId).HasColumnName("BA_ID");
            entity.Property(e => e.BaBloodGroup)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("BA_BloodGroup");
            entity.Property(e => e.BaBottlesAvailable).HasColumnName("BA_BottlesAvailable");
            entity.Property(e => e.BaDate)
                .HasColumnType("date")
                .HasColumnName("BA_Date");
            entity.Property(e => e.BaHId).HasColumnName("BA_H_ID");
            entity.Property(e => e.BaTime).HasColumnName("BA_Time");

            entity.HasOne(d => d.BaH).WithMany(p => p.BloodAvailabilities)
                .HasForeignKey(d => d.BaHId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Blood_Ava__BA_H___59FA5E80");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DId).HasName("PK__Doctors__76B8FF7DAC4F92E9");

            entity.Property(e => e.DId).HasColumnName("D_ID");
            entity.Property(e => e.DAvailablityStatus)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("D_AvailablityStatus");
            entity.Property(e => e.DEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("D_Email");
            entity.Property(e => e.DField)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("D_Field");
            entity.Property(e => e.DHId).HasColumnName("D_H_ID");
            entity.Property(e => e.DMobile)
                .HasMaxLength(11)
                .HasColumnName("D_Mobile");
            entity.Property(e => e.DName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("D_Name");
            entity.Property(e => e.DPassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("D_Password");

            entity.HasOne(d => d.DH).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.DHId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Doctors__D_H_ID__4D94879B");
        });

        modelBuilder.Entity<DoctorPrescription>(entity =>
        {
            entity.HasKey(e => e.DpId).HasName("PK__Doctor_P__7E732EA09FF169F0");

            entity.ToTable("Doctor_Prescription");

            entity.Property(e => e.DpId).HasColumnName("DP_ID");
            entity.Property(e => e.DpDId).HasColumnName("DP_D_ID");
            entity.Property(e => e.DpDate)
                .HasColumnType("datetime")
                .HasColumnName("DP_Date");
            entity.Property(e => e.DpDisease)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DP_Disease");
            entity.Property(e => e.DpPId).HasColumnName("DP_P_ID");

            entity.HasOne(d => d.DpD).WithMany(p => p.DoctorPrescriptions)
                .HasForeignKey(d => d.DpDId)
                .HasConstraintName("FK__Doctor_Pr__DP_D___5CD6CB2B");

            entity.HasOne(d => d.DpP).WithMany(p => p.DoctorPrescriptions)
                .HasForeignKey(d => d.DpPId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Doctor_Pr__DP_P___5DCAEF64");
        });

        modelBuilder.Entity<EmergencyContact>(entity =>
        {
            entity.HasKey(e => e.EcId).HasName("PK__Emergenc__46237E5951031F2D");

            entity.ToTable("Emergency_Contact");

            entity.Property(e => e.EcId).HasColumnName("EC_ID");
            entity.Property(e => e.EcHId).HasColumnName("EC_H_ID");
            entity.Property(e => e.EcNumber1)
                .HasMaxLength(12)
                .HasColumnName("EC_Number1");
            entity.Property(e => e.EcNumber2)
                .HasMaxLength(12)
                .HasColumnName("EC_Number2");

            entity.HasOne(d => d.EcH).WithMany(p => p.EmergencyContacts)
                .HasForeignKey(d => d.EcHId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Emergency__EC_H___6B24EA82");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FId).HasName("PK__Feedback__2C6EC7C3D9F307C6");

            entity.ToTable("Feedback");

            entity.Property(e => e.FId).HasColumnName("F_ID");
            entity.Property(e => e.FComments)
                .HasMaxLength(255)
                .HasColumnName("F_Comments");
            entity.Property(e => e.FDId).HasColumnName("F_D_ID");
            entity.Property(e => e.FPId).HasColumnName("F_P_ID");
            entity.Property(e => e.FRating).HasColumnName("F_Rating");

            entity.HasOne(d => d.FD).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.FDId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Feedback__F_D_ID__60A75C0F");

            entity.HasOne(d => d.FP).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.FPId)
                .HasConstraintName("FK__Feedback__F_P_ID__619B8048");
        });

        modelBuilder.Entity<HEmployee>(entity =>
        {
            entity.HasKey(e => e.HeId).HasName("PK__H_Employ__692EA79EF70025B5");

            entity.ToTable("H_Employee");

            entity.Property(e => e.HeId).HasColumnName("HE_ID");
            entity.Property(e => e.HeEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("HE_Email");
            entity.Property(e => e.HeHId).HasColumnName("HE_H_ID");
            entity.Property(e => e.HeName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("HE_Name");
            entity.Property(e => e.HePassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("HE_Password");
            entity.Property(e => e.HeRole)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("HE_Role");

            entity.HasOne(d => d.HeH).WithMany(p => p.HEmployees)
                .HasForeignKey(d => d.HeHId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__H_Employe__HE_H___571DF1D5");
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.HId).HasName("PK__Hospital__61F3893D78B274C2");

            entity.ToTable("Hospital");

            entity.Property(e => e.HId).HasColumnName("H_ID");
            entity.Property(e => e.HAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("H_Address");
            entity.Property(e => e.HEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("H_Email");
            entity.Property(e => e.HName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("H_Name");
            entity.Property(e => e.HPassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("H_Password");
            entity.Property(e => e.HlLatitude).HasColumnName("HL_Latitude");
            entity.Property(e => e.HlLongitude).HasColumnName("HL_Longitude");
        });

        modelBuilder.Entity<MedicalPortfolio>(entity =>
        {
            entity.HasKey(e => e.MpId).HasName("PK__Medical___D12FC71573D6D922");

            entity.ToTable("Medical_Portfolio");

            entity.Property(e => e.MpId).HasColumnName("MP_ID");
            entity.Property(e => e.MpDpId).HasColumnName("MP_DP_ID");
            entity.Property(e => e.MpPId).HasColumnName("MP_P_ID");
            entity.Property(e => e.MpPmId).HasColumnName("MP_PM_ID");

            entity.HasOne(d => d.MpDp).WithMany(p => p.MedicalPortfolios)
                .HasForeignKey(d => d.MpDpId)
                .HasConstraintName("FK__Medical_P__MP_DP__6E01572D");

            entity.HasOne(d => d.MpP).WithMany(p => p.MedicalPortfolios)
                .HasForeignKey(d => d.MpPId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Medical_P__MP_P___6EF57B66");

            entity.HasOne(d => d.MpPm).WithMany(p => p.MedicalPortfolios)
                .HasForeignKey(d => d.MpPmId)
                .HasConstraintName("FK__Medical_P__MP_PM__6FE99F9F");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PId).HasName("PK__Patients__A3420A77592C4340");

            entity.Property(e => e.PId).HasColumnName("P_ID");
            entity.Property(e => e.PAStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("P_A_Status");
            entity.Property(e => e.PDId).HasColumnName("P_D_ID");
            entity.Property(e => e.PDate)
                .HasColumnType("date")
                .HasColumnName("P_Date");
            entity.Property(e => e.PDob)
                .HasColumnType("date")
                .HasColumnName("P_DOB");
            entity.Property(e => e.PEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("P_Email");
            entity.Property(e => e.PMobile)
                .HasMaxLength(11)
                .HasColumnName("P_Mobile");
            entity.Property(e => e.PName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("P_Name");
            entity.Property(e => e.PPassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("P_Password");
            entity.Property(e => e.PReason)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("P_Reason");
            entity.Property(e => e.PTime).HasColumnName("P_Time");

            entity.HasOne(d => d.PD).WithMany(p => p.Patients)
                .HasForeignKey(d => d.PDId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Patients__P_D_ID__5441852A");
        });

        modelBuilder.Entity<PrescriptionMedication>(entity =>
        {
            entity.HasKey(e => e.PmId).HasName("PK__Prescrip__8E8EC70B6630E1AA");

            entity.ToTable("Prescription_Medications");

            entity.Property(e => e.PmId).HasColumnName("PM_ID");
            entity.Property(e => e.PmDosage)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PM_Dosage");
            entity.Property(e => e.PmDpId).HasColumnName("PM_DP_ID");
            entity.Property(e => e.PmEndDate)
                .HasColumnType("date")
                .HasColumnName("PM_EndDate");
            entity.Property(e => e.PmMedicine)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PM_Medicine");
            entity.Property(e => e.PmScheduleTime).HasColumnName("PM_ScheduleTime");
            entity.Property(e => e.PmStartDate)
                .HasColumnType("date")
                .HasColumnName("PM_StartDate");

            entity.HasOne(d => d.PmDp).WithMany(p => p.PrescriptionMedications)
                .HasForeignKey(d => d.PmDpId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Prescript__PM_DP__68487DD7");
        });

        modelBuilder.Entity<VideoConsultation>(entity =>
        {
            entity.HasKey(e => e.VcId).HasName("PK__Video_Co__FFB4AD255FFE2FE3");

            entity.ToTable("Video_Consultation");

            entity.Property(e => e.VcId).HasColumnName("VC_ID");
            entity.Property(e => e.VcDId).HasColumnName("VC_D_ID");
            entity.Property(e => e.VcDate)
                .HasColumnType("date")
                .HasColumnName("VC_Date");
            entity.Property(e => e.VcPId).HasColumnName("VC_P_ID");
            entity.Property(e => e.VcTime).HasColumnName("VC_Time");

            entity.HasOne(d => d.VcD).WithMany(p => p.VideoConsultations)
                .HasForeignKey(d => d.VcDId)
                .HasConstraintName("FK__Video_Con__VC_D___6477ECF3");

            entity.HasOne(d => d.VcP).WithMany(p => p.VideoConsultations)
                .HasForeignKey(d => d.VcPId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Video_Con__VC_P___656C112C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
