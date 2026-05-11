using System;
using ClinicAPI.Models.Dtos.AppointmentCategory;

namespace ClinicAPI.Services.AppointmentCategories;

public class AppointmentCategoryService : IAppointmentCategoryService
{

     public Task<bool> Exists(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentCategoryResponseDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AppointmentCategoryResponseDto> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
    public Task<AppointmentCategoryResponseDto> AddClinic(CreateAppointmentCategoryRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteClinic(int id)
    {
        throw new NotImplementedException();
    }

    public Task<AppointmentCategoryResponseDto> UpdateClinic(int id, UpdateAppointmentCategoryRequestDto request)
    {
        throw new NotImplementedException();
    }
}
