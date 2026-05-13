using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicAPI.Models.Dtos.Speciality;

public class UpdateSpecialityRequestDto
{
    public int Id { get; set; }

     [Required]
     [MinLength(1)]
    public string Name { get; set; }
}
