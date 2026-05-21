using System;
using ClinicAPI.Models.Dtos.AppointmentCategory;
using ClinicAPI.Services.Shared;

namespace ClinicAPI.Services.AppointmentCategories;

public interface IAppointmentCategoryService : IBaseService<AppointmentCategoryResponseDto, int>
{
    public Task<bool> Exists(int id);
    public Task<bool> HasAppointments(int id);
    public Task<AppointmentCategoryResponseDto> AddAppointmentCategory(CreateAppointmentCategoryRequestDto request);
    public Task UpdateAppointmentCategory(UpdateAppointmentCategoryRequestDto request);
    public Task DeleteAppointmentCategory(int id);
}
