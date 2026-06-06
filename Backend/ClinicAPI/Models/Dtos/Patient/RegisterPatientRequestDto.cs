using System.ComponentModel.DataAnnotations;
using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Patient;

public class RegisterPatientRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must be at least 8 characters with at least one uppercase letter, one lowercase letter, and one digit.")]
    public string Password { get; set; }

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

    [Required]
    public Religion Religion { get; set; }

    [Required]
    [RegularExpression(@"^\d{11}$",
        ErrorMessage = "Social security number must be exactly 11 digits.")]
    public string SocialSecurityNumber { get; set; }

    [Required]
    [MinLength(5)]
    [RegularExpression(@"^[A-Za-z0-9]+$",
        ErrorMessage = "Tax number must be alphanumeric.")]
    public string TaxNumber { get; set; }

    [Required]
    [MinLength(5)]
    [RegularExpression(@"^[A-Za-z0-9]+$",
        ErrorMessage = "Driver license number must be alphanumeric.")]
    public string DriverLicenseNumber { get; set; }

    [Required]
    [MinLength(5)]
    [RegularExpression(@"^[A-Za-z0-9]+$",
        ErrorMessage = "Medical insurance number must be alphanumeric.")]
    public string MedicalInsuranceNumber { get; set; }
}
