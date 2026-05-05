using System;

namespace ClinicAPI.Models.Entities;

public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsRegistered { get; set; }
    public string? Religion { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public string? MedicalInsuranceNumber { get; set; }
    public string? PasswordHash { get; set; }
    public ICollection<Appointment> Appointments { get; set; }


}
