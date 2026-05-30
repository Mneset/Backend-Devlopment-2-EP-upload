using System.ComponentModel.DataAnnotations;
using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Patient;

public class RegisterPatientRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
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
    [MinLength(1)]
    public string SocialSecurityNumber { get; set; }

    [Required]
    [MinLength(1)]
    public string TaxNumber { get; set; }

    [Required]
    [MinLength(1)]
    public string DriverLicenseNumber { get; set; }

    [Required]
    [MinLength(1)]
    public string MedicalInsuranceNumber { get; set; }
}
