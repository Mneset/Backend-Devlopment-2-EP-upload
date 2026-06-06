using System.ComponentModel.DataAnnotations;
using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Patient;

public class UpdatePatientRequestDto
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(1)]
    public string FirstName { get; set; }

    [Required]
    [MinLength(1)]
    public string LastName { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    public Religion? Religion { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public string? MedicalInsuranceNumber { get; set; }
}
