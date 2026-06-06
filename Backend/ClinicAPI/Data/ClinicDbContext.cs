using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClinicAPI.Data;

public class ClinicDbContext : DbContext
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentCategory> AppointmentCategories { get; set; }
    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Speciality> Specialities { get; set; }


    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Clinic)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.ClinicId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Clinic>().HasData(
           new Clinic { Id = 1, Name = "Oslo University Hospital", IsActive = true },
           new Clinic { Id = 2, Name = "Ullevål Medical Center", IsActive = true },
           new Clinic { Id = 3, Name = "Rikshospitalet Clinic", IsActive = true },
           new Clinic { Id = 4, Name = "Drammen Health Clinic", IsActive = true },
           new Clinic { Id = 5, Name = "Ahus Outpatient Clinic", IsActive = true }
       );

        modelBuilder.Entity<Speciality>().HasData(
            new Speciality { Id = 1, Name = "Cardiology" },
            new Speciality { Id = 2, Name = "Dermatology" },
            new Speciality { Id = 3, Name = "Neurology" },
            new Speciality { Id = 4, Name = "Orthopedics" }
        );

        modelBuilder.Entity<AppointmentCategory>().HasData(
            new AppointmentCategory { Id = 1, Name = "General Checkup" },
            new AppointmentCategory { Id = 2, Name = "Follow-up Consultation" },
            new AppointmentCategory { Id = 3, Name = "Specialist Referral" },
            new AppointmentCategory { Id = 4, Name = "Vaccination" },
            new AppointmentCategory { Id = 5, Name = "Lab Test" }
        );

        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { Id = 1, FirstName = "Erik", LastName = "Hansen", IsActive = true, SpecialityId = 1, ClinicId = 1, Gender = Gender.Male },
            new Doctor { Id = 2, FirstName = "Anna", LastName = "Olsen", IsActive = true, SpecialityId = 2, ClinicId = 2, Gender = Gender.Female },
            new Doctor { Id = 3, FirstName = "Lars", LastName = "Berg", IsActive = true, SpecialityId = 3, ClinicId = 3, Gender = Gender.Male },
            new Doctor { Id = 4, FirstName = "Sofia", LastName = "Dahl", IsActive = true, SpecialityId = 4, ClinicId = 4, Gender = Gender.Female }
        );

        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id = 1, FirstName = "Jonas", LastName = "Nilsen", Gender = Gender.Male, DateOfBirth = new DateTime(1990, 5, 15), IsActive = true, IsRegistered = true, Religion = Religion.PreferNotToSay, SocialSecurityNumber = "15059012345", TaxNumber = "TX10001", DriverLicenseNumber = "DL10001", MedicalInsuranceNumber = "MI10001" },
            new Patient { Id = 2, FirstName = "Marie", LastName = "Andersen", Gender = Gender.Female, DateOfBirth = new DateTime(1985, 3, 22), IsActive = true, IsRegistered = true, Religion = Religion.PreferNotToSay, SocialSecurityNumber = "22038512345", TaxNumber = "TX10002", DriverLicenseNumber = "DL10002", MedicalInsuranceNumber = "MI10002" },
            new Patient { Id = 3, FirstName = "Guest", LastName = "User", Gender = Gender.Male, DateOfBirth = new DateTime(2000, 1, 1), IsActive = true, IsRegistered = false }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Email = "admin@clinic.com", PasswordHash = "$2a$11$4BEOTWkbHvboS6Nc9uM2P.rpQjvxmH5QuMcUzOp7Gl/EBys5hA2hK", Role = Role.Admin, PatientId = null },
            new User { Id = 2, Email = "jonas@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 1 },
            new User { Id = 3, Email = "marie@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 2 },
            new User { Id = 4, Email = "guest@email.com", PasswordHash = null, Role = Role.Guest, PatientId = 3 }
        );

        modelBuilder.Entity<Appointment>().HasData(
            new Appointment { Id = 1, PatientId = 1, DoctorId = 1, ClinicId = 1, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 6, 10, 9, 0, 0), DurationInMin = 30, Status = Status.Scheduled },
            new Appointment { Id = 2, PatientId = 2, DoctorId = 2, ClinicId = 2, AppointmentCategoryId = 2, AppointmentDate = new DateTime(2026, 6, 11, 10, 0, 0), DurationInMin = 45, Status = Status.Scheduled },
            new Appointment { Id = 3, PatientId = 1, DoctorId = 3, ClinicId = 3, AppointmentCategoryId = 3, AppointmentDate = new DateTime(2026, 5, 1, 14, 0, 0), DurationInMin = 60, Status = Status.Completed }
        );
    }

}
