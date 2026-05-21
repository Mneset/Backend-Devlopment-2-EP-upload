using System.ComponentModel.DataAnnotations;
using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Doctor;

public class CreateDoctorRequestDto
{
    [Required]
    [MinLength(1)]
    public string FirstName { get; set; }

    [Required]
    [MinLength(1)]
    public string LastName { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public int SpecialityId { get; set; }

    [Required]
    public int ClinicId { get; set; }
}
