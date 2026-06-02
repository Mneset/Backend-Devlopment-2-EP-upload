using ClinicAPI.Data;
using ClinicAPI.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicAPI.Tests;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ClinicDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ClinicDbContext>(options =>
                options.UseInMemoryDatabase("ClinicTestDb"));
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
        db.Database.EnsureCreated();
        SeedDatabase(db);
    }

    private static void SeedDatabase(ClinicDbContext db)
    {
        db.Appointments.RemoveRange(db.Appointments);
        db.Users.RemoveRange(db.Users);
        db.Patients.RemoveRange(db.Patients);
        db.Doctors.RemoveRange(db.Doctors);
        db.AppointmentCategories.RemoveRange(db.AppointmentCategories);
        db.Specialities.RemoveRange(db.Specialities);
        db.Clinics.RemoveRange(db.Clinics);

        db.SaveChanges();

        db.Clinics.AddRange(
            new Clinic { Id = 1, Name = "Oslo University Hospital", IsActive = true },
            new Clinic { Id = 2, Name = "Ullevål Medical Center", IsActive = true },
            new Clinic { Id = 3, Name = "Rikshospitalet Clinic", IsActive = true },
            new Clinic { Id = 4, Name = "Drammen Health Clinic", IsActive = true },
            new Clinic { Id = 5, Name = "Ahus Outpatient Clinic", IsActive = true }
        );

        db.Specialities.AddRange(
            new Speciality { Id = 1, Name = "Cardiology" },
            new Speciality { Id = 2, Name = "Dermatology" },
            new Speciality { Id = 3, Name = "Neurology" },
            new Speciality { Id = 4, Name = "Orthopedics" }
        );

        db.AppointmentCategories.AddRange(
            new AppointmentCategory { Id = 1, Name = "General Checkup" },
            new AppointmentCategory { Id = 2, Name = "Follow-up Consultation" },
            new AppointmentCategory { Id = 3, Name = "Specialist Referral" },
            new AppointmentCategory { Id = 4, Name = "Vaccination" },
            new AppointmentCategory { Id = 5, Name = "Lab Test" }
        );

        db.Doctors.AddRange(
            new Doctor { Id = 1, FirstName = "Erik", LastName = "Hansen", IsActive = true, SpecialityId = 1, ClinicId = 1, Gender = Gender.Male },
            new Doctor { Id = 2, FirstName = "Anna", LastName = "Olsen", IsActive = true, SpecialityId = 2, ClinicId = 2, Gender = Gender.Female },
            new Doctor { Id = 3, FirstName = "Lars", LastName = "Berg", IsActive = true, SpecialityId = 3, ClinicId = 3, Gender = Gender.Male },
            new Doctor { Id = 4, FirstName = "Sofia", LastName = "Dahl", IsActive = true, SpecialityId = 4, ClinicId = 4, Gender = Gender.Female }
        );

        db.Patients.AddRange(
            new Patient { Id = 1, FirstName = "Jonas", LastName = "Nilsen", Gender = Gender.Male, DateOfBirth = new DateTime(1990, 5, 15), IsActive = true, IsRegistered = true },
            new Patient { Id = 2, FirstName = "Marie", LastName = "Andersen", Gender = Gender.Female, DateOfBirth = new DateTime(1985, 3, 22), IsActive = true, IsRegistered = true },
            new Patient { Id = 3, FirstName = "Guest", LastName = "User", Gender = Gender.Male, DateOfBirth = new DateTime(2000, 1, 1), IsActive = true, IsRegistered = false }
        );

        db.Users.AddRange(
            new User { Id = 1, Email = "admin@clinic.com", PasswordHash = "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq", Role = Role.Admin, PatientId = null },
            new User { Id = 2, Email = "jonas@email.com", PasswordHash = "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq", Role = Role.Patient, PatientId = 1 },
            new User { Id = 3, Email = "marie@email.com", PasswordHash = "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq", Role = Role.Patient, PatientId = 2 },
            new User { Id = 4, Email = "guest@email.com", PasswordHash = null, Role = Role.Guest, PatientId = 3 }
        );

        db.Appointments.AddRange(
            new Appointment { Id = 1, PatientId = 1, DoctorId = 1, ClinicId = 1, AppointmentCategoryId = 1, AppointmentDate = new DateTime(2026, 6, 10, 9, 0, 0), DurationInMin = 30, Status = Status.Scheduled },
            new Appointment { Id = 2, PatientId = 2, DoctorId = 2, ClinicId = 2, AppointmentCategoryId = 2, AppointmentDate = new DateTime(2026, 6, 11, 10, 0, 0), DurationInMin = 45, Status = Status.Scheduled },
            new Appointment { Id = 3, PatientId = 1, DoctorId = 3, ClinicId = 3, AppointmentCategoryId = 3, AppointmentDate = new DateTime(2026, 5, 1, 14, 0, 0), DurationInMin = 60, Status = Status.Completed }
        );

        db.SaveChanges();
    }
}