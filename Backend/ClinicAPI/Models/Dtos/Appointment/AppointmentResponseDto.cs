using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Appointment;

public class AppointmentResponseDto
{
    public int Id { get; set; }
    public Status Status { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int DurationInMin { get; set; }
    public int PatientId { get; set; }
    public string PatientFirstName { get; set; }
    public string PatientLastName { get; set; }
    public int DoctorId { get; set; }
    public string DoctorFirstName { get; set; }
    public string DoctorLastName { get; set; }
    public int ClinicId { get; set; }
    public string ClinicName { get; set; }
    public int AppointmentCategoryId { get; set; }
    public string AppointmentCategoryName { get; set; }
}
