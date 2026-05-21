using ClinicAPI.Models.Dtos.Doctor;
using ClinicAPI.Services.Shared;

namespace ClinicAPI.Services.Doctors;

public interface IDoctorService : IBaseService<DoctorResponseDto, int>
{
    public Task<bool> Exists(int id);
    public Task<IEnumerable<DoctorResponseDto>> GetAllIncludingInactiveAsync();
    public Task<DoctorResponseDto?> GetByIdIncludingInactiveAsync(int id);
    public Task<IEnumerable<DoctorResponseDto>> SearchByNameAsync(string name);
    public Task<DoctorResponseDto> AddDoctor(CreateDoctorRequestDto request);
    public Task UpdateDoctor(UpdateDoctorRequestDto request);
    public Task DeactivateDoctor(int id);
    public Task ReactivateDoctor(int id);
}
