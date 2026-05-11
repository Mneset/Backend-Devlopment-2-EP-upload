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
            new Clinic{ Id = 1, Name = "First Clinic"},
            new Clinic{ Id = 2, Name = "Second Clinic"},
            new Clinic{ Id = 3, Name = "Third Clinic"}
        );
    }

}
