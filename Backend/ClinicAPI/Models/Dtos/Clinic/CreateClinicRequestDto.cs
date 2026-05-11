using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicAPI.Models.Dtos.Clinic;

public class CreateClinicRequestDto
{
     [Required]
     [MinLength(1)]
     public string Name { get; set; }
}
