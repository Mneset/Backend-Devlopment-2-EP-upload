namespace ClinicAPI.Models.Entities;

public class Clinic
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Doctor> Doctors { get; set; }
    public ICollection<Appointment> Appointments { get; set; }

}