using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Doctor;

public class DoctorResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public bool IsActive { get; set; }
    public int SpecialityId { get; set; }
    public string SpecialityName { get; set; }
    public int ClinicId { get; set; }
    public string ClinicName { get; set; }
}
