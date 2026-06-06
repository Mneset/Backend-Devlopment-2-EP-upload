using System;
using ClinicAPI.Data;
using ClinicAPI.Models.Dtos.AppointmentCategory;
using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;



namespace ClinicAPI.Services.AppointmentCategories;

public class AppointmentCategoryService : IAppointmentCategoryService
{
     private readonly ClinicDbContext _context;

        public AppointmentCategoryService(ClinicDbContext context)
        {
            _context = context;
        }

     public async Task<bool> Exists(int id)
    {
        return await _context.AppointmentCategories.AnyAsync(ac => ac.Id == id);
    }

    public async Task<bool> HasAppointments(int id)
    {
        return await _context.Appointments.AnyAsync(a => a.AppointmentCategoryId == id);
    }

    public async Task<IEnumerable<AppointmentCategoryResponseDto>> GetAllAsync()
    {
        return await _context.AppointmentCategories
            .Select(ac => new AppointmentCategoryResponseDto {Id = ac.Id, Name = ac.Name })
            .ToListAsync();
    }

    public async Task<AppointmentCategoryResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.AppointmentCategories.FirstOrDefaultAsync(ac => ac.Id == id);
        if(entity == null) return null;
        return new AppointmentCategoryResponseDto { Id = entity.Id, Name = entity.Name };
    }
    public async Task<AppointmentCategoryResponseDto> AddAppointmentCategory(CreateAppointmentCategoryRequestDto appointmentCategoryData)
    {
        var entity = new AppointmentCategory { Name = appointmentCategoryData.Name };
       _context.AppointmentCategories.Add(entity);
       await _context.SaveChangesAsync();
       return new AppointmentCategoryResponseDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task DeleteAppointmentCategory(int id)
    {
        var category = await _context.AppointmentCategories.FirstOrDefaultAsync(ac => ac.Id == id);
        if (category == null) return;
        _context.AppointmentCategories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAppointmentCategory(UpdateAppointmentCategoryRequestDto appointmentCategoryData)
    {
        var entity = await _context.AppointmentCategories.FirstAsync(ac => ac.Id == appointmentCategoryData.Id);
        entity.Name = appointmentCategoryData.Name;
        await _context.SaveChangesAsync();
    }
}
