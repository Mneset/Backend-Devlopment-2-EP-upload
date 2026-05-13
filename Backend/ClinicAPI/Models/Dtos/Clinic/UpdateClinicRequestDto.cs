using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicAPI.Models.Dtos.Clinic;

public class UpdateClinicRequestDto
{
     public int Id { get; set; }
     
     [Required]
     [MinLength(1)]
     public string Name { get; set; }
}
