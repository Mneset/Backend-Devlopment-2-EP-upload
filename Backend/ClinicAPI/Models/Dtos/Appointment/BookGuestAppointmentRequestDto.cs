using System.ComponentModel.DataAnnotations;
using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Appointment;

public class BookGuestAppointmentRequestDto
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
    public DateTime DateOfBirth { get; set; }

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
