using ClinicAPI.Services.Shared;
using ClinicAPI.Models.Dtos.Clinic;

namespace ClinicAPI.Services.Clinics;

public interface IClinicService : IBaseService<ClinicResponseDto, int>
{
    public Task<bool> Exists(int id);
    public Task<ClinicResponseDto> AddClinic(CreateClinicRequestDto request);
    public Task UpdateClinic(UpdateClinicRequestDto request);
    public Task DeleteClinic(int id);
}
