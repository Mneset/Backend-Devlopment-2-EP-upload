using System.ComponentModel.DataAnnotations;

namespace ClinicAPI.Models.Dtos.AppointmentCategory;

public class UpdateAppointmentCategoryRequestDto
{
    public int Id { get; set; }
    
     [Required]
     [MinLength(1)]
    public string Name { get; set; }
}
