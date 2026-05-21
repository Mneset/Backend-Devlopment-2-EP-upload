using ClinicAPI.Models.Dtos.Speciality;
using ClinicAPI.Services.Shared;

namespace ClinicAPI.Services.Specialities;

public interface ISpecialityService : IBaseService< SpecialityResponseDto, int >
{
    public Task<bool> Exists(int id);
    public Task<bool> HasDoctors(int id);
    public Task<SpecialityResponseDto> AddSpeciality(CreateSpecialityRequestDto request);
    public Task UpdateSpeciality(UpdateSpecialityRequestDto request);
    public Task DeleteSpeciality(int id);
}
