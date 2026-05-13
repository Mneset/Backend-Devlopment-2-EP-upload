using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicAPI.Models.Dtos.AppointmentCategory;

public class CreateAppointmentCategoryRequestDto
{
     [Required]
     [MinLength(1)]
    public string Name { get; set; }
}
