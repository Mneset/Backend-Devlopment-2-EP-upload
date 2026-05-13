using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClinicAPI.Data;

public class ClinicDbContext : DbContext
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentCategory> AppointmentCategories { get; set; }
    public DbSet<Clinic> Clinic { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Speciality> Specialities { get; set; }


    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*
            Appoinment links to both Doctor and Clinic, but Doctor also links to Clinic
            This creates multiple delete paths to Appoinment and SQL Server does not allow that.
            Therefore I'm defining these two relationships explicitly so i can modify the delete behaviour and disable cascade delete.
            The rest of the relationships are handled automatically by EF
        */

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

        modelBuilder.Entity<Clinic>().HasData(
            new Clinic { Id = 1, Name = "Oslo University Hospital" },
            new Clinic { Id = 2, Name = "Ullevål Medical Center" },
            new Clinic { Id = 3, Name = "Rikshospitalet Clinic" },
            new Clinic { Id = 4, Name = "Drammen Health Clinic" },
            new Clinic { Id = 5, Name = "Ahus Outpatient Clinic" }
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
    }

}
