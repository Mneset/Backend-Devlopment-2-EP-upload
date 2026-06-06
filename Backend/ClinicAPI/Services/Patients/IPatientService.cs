using ClinicAPI.Models.Dtos.Patient;
using ClinicAPI.Services.Shared;

namespace ClinicAPI.Services.Patients;

public interface IPatientService : IBaseService<PatientResponseDto, int>
{
    public Task<bool> Exists(int id);
    public Task<IEnumerable<PatientResponseDto>> GetAllIncludingInactiveAsync();
    public Task<PatientResponseDto?> GetByIdIncludingInactiveAsync(int id);
    public Task<PatientResponseDto> RegisterPatient(RegisterPatientRequestDto request);
    public Task<PatientResponseDto> CreateGuestPatient(CreateGuestPatientRequestDto request);
    public Task UpdatePatient(UpdatePatientRequestDto request);
    public Task UpdateOwnProfile(int patientId, UpdateOwnProfileRequestDto request);
    public Task DeactivatePatient(int id);
    public Task ReactivatePatient(int id);
}
