using System;

namespace ClinicAPI.Models.Entities;

public enum Status
{
    Scheduled,
    Completed,
    Cancelled
}
public class Appointment
{
    public int Id { get; set; }
    public Status Status { get; set; } = Status.Scheduled;
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int ClinicId { get; set; }
    public int AppointmentCategoryId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int DurationInMin { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public Clinic Clinic { get; set; }
    public AppointmentCategory AppointmentCategory { get; set; }
}
