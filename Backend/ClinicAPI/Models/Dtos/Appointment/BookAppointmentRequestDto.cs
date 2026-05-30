using System.ComponentModel.DataAnnotations;

namespace ClinicAPI.Models.Dtos.Appointment;

public class BookAppointmentRequestDto
{
    [Required]
    public int DoctorId { get; set; }

    [Required]
    public int ClinicId { get; set; }

    [Required]
    public int AppointmentCategoryId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [Range(1, 480)]
    public int DurationInMin { get; set; }
}
