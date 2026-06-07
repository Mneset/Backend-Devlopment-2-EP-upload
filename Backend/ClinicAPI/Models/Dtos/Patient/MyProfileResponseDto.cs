using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Patient;

public class MyProfileResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Religion? Religion { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public string? MedicalInsuranceNumber { get; set; }
}
