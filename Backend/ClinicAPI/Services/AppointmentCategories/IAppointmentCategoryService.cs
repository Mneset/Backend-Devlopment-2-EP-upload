using System;
using ClinicAPI.Models.Dtos.AppointmentCategory;
using ClinicAPI.Services.Shared;

namespace ClinicAPI.Services.AppointmentCategories;

public interface IAppointmentCategoryService : IBaseService<AppointmentCategoryResponseDto, int>
{
    public Task<bool> Exists(int id);
    public Task<AppointmentCategoryResponseDto> AddClinic(CreateAppointmentCategoryRequestDto request);
    public Task<AppointmentCategoryResponseDto> UpdateClinic(int id, UpdateAppointmentCategoryRequestDto request);
    public Task DeleteClinic(int id);
}
