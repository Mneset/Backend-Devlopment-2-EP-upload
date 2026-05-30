using System.ComponentModel.DataAnnotations;

namespace ClinicAPI.Models.Dtos.Appointment;

public class RescheduleAppointmentRequestDto
{
    public int Id { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [Range(1, 480)]
    public int DurationInMin { get; set; }
}
