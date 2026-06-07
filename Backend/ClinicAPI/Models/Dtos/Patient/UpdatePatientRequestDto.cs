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

    [RegularExpression(@"^\d{11}$",
        ErrorMessage = "Social security number must be exactly 11 digits.")]
    public string? SocialSecurityNumber { get; set; }

    [MinLength(5)]
    [RegularExpression(@"^[A-Za-z0-9]+$",
        ErrorMessage = "Tax number must be alphanumeric.")]
    public string? TaxNumber { get; set; }

    [MinLength(5)]
    [RegularExpression(@"^[A-Za-z0-9]+$",
        ErrorMessage = "Driver license number must be alphanumeric.")]
    public string? DriverLicenseNumber { get; set; }

    [MinLength(5)]
    [RegularExpression(@"^[A-Za-z0-9]+$",
        ErrorMessage = "Medical insurance number must be alphanumeric.")]
    public string? MedicalInsuranceNumber { get; set; }
}
