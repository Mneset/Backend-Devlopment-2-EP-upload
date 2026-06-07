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
            new Doctor { Id = 4, FirstName = "Sofia", LastName = "Dahl", IsActive = true, SpecialityId = 4, ClinicId = 4, Gender = Gender.Female },
            new Doctor { Id = 5, FirstName = "Olav", LastName = "Lund", IsActive = true, SpecialityId = 1, ClinicId = 2, Gender = Gender.Male },
            new Doctor { Id = 6, FirstName = "Ingrid", LastName = "Solberg", IsActive = true, SpecialityId = 1, ClinicId = 5, Gender = Gender.Female },
            new Doctor { Id = 7, FirstName = "Maria", LastName = "Strand", IsActive = true, SpecialityId = 2, ClinicId = 4, Gender = Gender.Female },
            new Doctor { Id = 8, FirstName = "Henrik", LastName = "Vik", IsActive = true, SpecialityId = 2, ClinicId = 1, Gender = Gender.Male },
            new Doctor { Id = 9, FirstName = "Astrid", LastName = "Bakke", IsActive = true, SpecialityId = 3, ClinicId = 5, Gender = Gender.Female },
            new Doctor { Id = 10, FirstName = "Tor", LastName = "Iversen", IsActive = true, SpecialityId = 4, ClinicId = 2, Gender = Gender.Male },
            new Doctor { Id = 11, FirstName = "Linnea", LastName = "Holm", IsActive = true, SpecialityId = 4, ClinicId = 3, Gender = Gender.Female },
            new Doctor { Id = 12, FirstName = "Magnus", LastName = "Eide", IsActive = true, SpecialityId = 3, ClinicId = 1, Gender = Gender.Male }
        );

        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id = 1, FirstName = "Jonas", LastName = "Nilsen", Gender = Gender.Male, DateOfBirth = new DateTime(1990, 5, 15), IsActive = true, IsRegistered = true, Religion = Religion.PreferNotToSay, SocialSecurityNumber = "15059012345", TaxNumber = "TX10001", DriverLicenseNumber = "DL10001", MedicalInsuranceNumber = "MI10001" },
            new Patient { Id = 2, FirstName = "Marie", LastName = "Andersen", Gender = Gender.Female, DateOfBirth = new DateTime(1985, 3, 22), IsActive = true, IsRegistered = true, Religion = Religion.PreferNotToSay, SocialSecurityNumber = "22038512345", TaxNumber = "TX10002", DriverLicenseNumber = "DL10002", MedicalInsuranceNumber = "MI10002" },
            new Patient { Id = 3, FirstName = "Guest", LastName = "User", Gender = Gender.Male, DateOfBirth = new DateTime(2000, 1, 1), IsActive = true, IsRegistered = false },
            new Patient { Id = 4, FirstName = "Henrik", LastName = "Aas", Gender = Gender.Male, DateOfBirth = new DateTime(1988, 4, 12), IsActive = true, IsRegistered = true, Religion = Religion.Christianity, SocialSecurityNumber = "12048812345", TaxNumber = "TX10004", DriverLicenseNumber = "DL10004", MedicalInsuranceNumber = "MI10004" },
            new Patient { Id = 5, FirstName = "Kari", LastName = "Solheim", Gender = Gender.Female, DateOfBirth = new DateTime(1995, 11, 3), IsActive = true, IsRegistered = true, Religion = Religion.None, SocialSecurityNumber = "03119512345", TaxNumber = "TX10005", DriverLicenseNumber = "DL10005", MedicalInsuranceNumber = "MI10005" },
            new Patient { Id = 6, FirstName = "Ola", LastName = "Lien", Gender = Gender.Male, DateOfBirth = new DateTime(1972, 7, 28), IsActive = true, IsRegistered = true, Religion = Religion.PreferNotToSay, SocialSecurityNumber = "28077212345", TaxNumber = "TX10006", DriverLicenseNumber = "DL10006", MedicalInsuranceNumber = "MI10006" },
            new Patient { Id = 7, FirstName = "Ingvild", LastName = "Moen", Gender = Gender.Female, DateOfBirth = new DateTime(1992, 2, 19), IsActive = true, IsRegistered = true, Religion = Religion.Other, SocialSecurityNumber = "19029212345", TaxNumber = "TX10007", DriverLicenseNumber = "DL10007", MedicalInsuranceNumber = "MI10007" },
            new Patient { Id = 8, FirstName = "Sven", LastName = "Berge", Gender = Gender.Male, DateOfBirth = new DateTime(1980, 9, 5), IsActive = true, IsRegistered = true, Religion = Religion.Christianity, SocialSecurityNumber = "05098012345", TaxNumber = "TX10008", DriverLicenseNumber = "DL10008", MedicalInsuranceNumber = "MI10008" },
            new Patient { Id = 9, FirstName = "Astrid", LastName = "Kvam", Gender = Gender.Female, DateOfBirth = new DateTime(1998, 12, 30), IsActive = true, IsRegistered = true, Religion = Religion.None, SocialSecurityNumber = "30129812345", TaxNumber = "TX10009", DriverLicenseNumber = "DL10009", MedicalInsuranceNumber = "MI10009" },
            new Patient { Id = 10, FirstName = "Per", LastName = "Walk-in", Gender = Gender.Male, DateOfBirth = new DateTime(1965, 3, 14), IsActive = true, IsRegistered = false },
            new Patient { Id = 11, FirstName = "Lisa", LastName = "Visitor", Gender = Gender.Female, DateOfBirth = new DateTime(1978, 8, 22), IsActive = true, IsRegistered = false },
            new Patient { Id = 12, FirstName = "Thomas", LastName = "Drop-in", Gender = Gender.Male, DateOfBirth = new DateTime(2002, 6, 9), IsActive = true, IsRegistered = false }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Email = "admin@clinic.com", PasswordHash = "$2a$11$4BEOTWkbHvboS6Nc9uM2P.rpQjvxmH5QuMcUzOp7Gl/EBys5hA2hK", Role = Role.Admin, PatientId = null },
            new User { Id = 2, Email = "jonas@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 1 },
            new User { Id = 3, Email = "marie@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 2 },
            new User { Id = 4, Email = "guest@email.com", PasswordHash = null, Role = Role.Guest, PatientId = 3 },
            new User { Id = 5, Email = "henrik@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 4 },
            new User { Id = 6, Email = "kari@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 5 },
            new User { Id = 7, Email = "ola@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 6 },
            new User { Id = 8, Email = "ingvild@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 7 },
            new User { Id = 9, Email = "sven@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 8 },
            new User { Id = 10, Email = "astrid@email.com", PasswordHash = "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", Role = Role.Patient, PatientId = 9 }
        );

        modelBuilder.Entity<Appointment>().HasData(
            new Appointment { Id = 1, PatientId = 1, DoctorId = 1, ClinicId = 1, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 6, 10, 9, 0, 0), DurationInMin = 30, Status = Status.Scheduled },
            new Appointment { Id = 2, PatientId = 2, DoctorId = 2, ClinicId = 2, AppointmentCategoryId = 2, AppointmentDate = new DateTime(2026, 6, 11, 10, 0, 0), DurationInMin = 45, Status = Status.Scheduled },
            new Appointment { Id = 3, PatientId = 1, DoctorId = 3, ClinicId = 3, AppointmentCategoryId = 3, AppointmentDate = new DateTime(2026, 5, 1, 14, 0, 0), DurationInMin = 60, Status = Status.Completed },
            new Appointment { Id = 4, PatientId = 1, DoctorId = 5, ClinicId = 2, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 6, 15, 10, 0, 0), DurationInMin = 30, Status = Status.Scheduled },
            new Appointment { Id = 5, PatientId = 4, DoctorId = 1, ClinicId = 1, AppointmentCategoryId = 2, AppointmentDate = new DateTime(2026, 6, 20, 11, 0, 0), DurationInMin = 45, Status = Status.Scheduled },
            new Appointment { Id = 6, PatientId = 5, DoctorId = 8, ClinicId = 1, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 6, 25, 14, 0, 0), DurationInMin = 30, Status = Status.Scheduled },
            new Appointment { Id = 7, PatientId = 6, DoctorId = 11, ClinicId = 3, AppointmentCategoryId = 3, AppointmentDate = new DateTime(2026, 7, 2, 9, 0, 0), DurationInMin = 60, Status = Status.Scheduled },
            new Appointment { Id = 8, PatientId = 7, DoctorId = 9, ClinicId = 5, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 7, 10, 13, 30, 0), DurationInMin = 30, Status = Status.Scheduled },
            new Appointment { Id = 9, PatientId = 8, DoctorId = 4, ClinicId = 4, AppointmentCategoryId = 4, AppointmentDate = new DateTime(2026, 7, 15, 16, 0, 0), DurationInMin = 15, Status = Status.Scheduled },
            new Appointment { Id = 10, PatientId = 9, DoctorId = 2, ClinicId = 2, AppointmentCategoryId = 5, AppointmentDate = new DateTime(2026, 7, 20, 10, 30, 0), DurationInMin = 30, Status = Status.Scheduled },
            new Appointment { Id = 11, PatientId = 4, DoctorId = 12, ClinicId = 1, AppointmentCategoryId = 2, AppointmentDate = new DateTime(2026, 5, 15, 9, 0, 0), DurationInMin = 45, Status = Status.Scheduled },
            new Appointment { Id = 12, PatientId = 2, DoctorId = 10, ClinicId = 2, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 4, 10, 11, 0, 0), DurationInMin = 30, Status = Status.Completed },
            new Appointment { Id = 13, PatientId = 5, DoctorId = 6, ClinicId = 5, AppointmentCategoryId = 5, AppointmentDate = new DateTime(2026, 3, 15, 14, 0, 0), DurationInMin = 45, Status = Status.Completed },
            new Appointment { Id = 14, PatientId = 1, DoctorId = 1, ClinicId = 1, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 2, 20, 10, 0, 0), DurationInMin = 30, Status = Status.Completed },
            new Appointment { Id = 15, PatientId = 6, DoctorId = 4, ClinicId = 4, AppointmentCategoryId = 3, AppointmentDate = new DateTime(2026, 1, 15, 13, 0, 0), DurationInMin = 60, Status = Status.Completed },
            new Appointment { Id = 16, PatientId = 8, DoctorId = 7, ClinicId = 4, AppointmentCategoryId = 4, AppointmentDate = new DateTime(2026, 4, 25, 15, 30, 0), DurationInMin = 15, Status = Status.Completed },
            new Appointment { Id = 17, PatientId = 7, DoctorId = 5, ClinicId = 2, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 5, 5, 10, 0, 0), DurationInMin = 30, Status = Status.Cancelled },
            new Appointment { Id = 18, PatientId = 9, DoctorId = 11, ClinicId = 3, AppointmentCategoryId = 2, AppointmentDate = new DateTime(2026, 6, 28, 14, 30, 0), DurationInMin = 45, Status = Status.Cancelled },
            new Appointment { Id = 19, PatientId = 2, DoctorId = 3, ClinicId = 3, AppointmentCategoryId = 5, AppointmentDate = new DateTime(2026, 4, 18, 16, 0, 0), DurationInMin = 30, Status = Status.Cancelled },
            new Appointment { Id = 20, PatientId = 10, DoctorId = 4, ClinicId = 4, AppointmentCategoryId = 4, AppointmentDate = new DateTime(2026, 6, 12, 11, 30, 0), DurationInMin = 15, Status = Status.Scheduled },
            new Appointment { Id = 21, PatientId = 11, DoctorId = 2, ClinicId = 2, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 4, 1, 9, 0, 0), DurationInMin = 30, Status = Status.Completed },
            new Appointment { Id = 22, PatientId = 12, DoctorId = 6, ClinicId = 5, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 6, 30, 11, 0, 0), DurationInMin = 30, Status = Status.Scheduled }
        );
    }

}
