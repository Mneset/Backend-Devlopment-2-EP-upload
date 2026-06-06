using System;

namespace ClinicAPI.Models.Entities;

public enum Gender
{
    Male,
    Female
}

public enum Religion
{
    Christianity,
    Islam,
    Judaism,
    Hinduism,
    Buddhism,
    Other,
    None,
    PreferNotToSay
}

public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public bool IsRegistered { get; set; }
    public Religion? Religion { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public string? MedicalInsuranceNumber { get; set; }
    public ICollection<Appointment> Appointments { get; set; }


}
