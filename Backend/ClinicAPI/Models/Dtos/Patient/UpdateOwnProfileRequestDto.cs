using System.ComponentModel.DataAnnotations;
using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Patient;

public class UpdateOwnProfileRequestDto
{
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
}
