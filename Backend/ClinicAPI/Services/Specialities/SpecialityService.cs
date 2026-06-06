using ClinicAPI.Data;
using ClinicAPI.Models.Dtos.Speciality;
using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAPI.Services.Specialities;

public class SpecialityService : ISpecialityService
{
    private readonly ClinicDbContext _context;

        public SpecialityService(ClinicDbContext context)
        {
            _context = context;
        }

    public async Task<bool> Exists(int id)
    {
        return await _context.Specialities.AnyAsync(s => s.Id == id);

    }

    public async Task<bool> HasDoctors(int id)
    {
        return await _context.Doctors.AnyAsync(d => d.SpecialityId == id);
    }

    public async Task<IEnumerable<SpecialityResponseDto>> GetAllAsync()
    {
        return await _context.Specialities
            .Select(s => new SpecialityResponseDto {Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<SpecialityResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);
        if(entity == null) return null;
        return new SpecialityResponseDto { Id = entity.Id, Name = entity.Name };
    }
    public async Task<SpecialityResponseDto> AddSpeciality(CreateSpecialityRequestDto speciality)
    {
        var entity = new Speciality { Name = speciality.Name };
       _context.Specialities.Add(entity);
       await _context.SaveChangesAsync();
       return new SpecialityResponseDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task DeleteSpeciality(int id)
    {
        var speciality = await _context.Specialities.FirstOrDefaultAsync(s => s.Id == id);
        if (speciality == null) return;
        _context.Specialities.Remove(speciality);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSpeciality(UpdateSpecialityRequestDto speciality)
    {
        var entity = await _context.Specialities.FirstAsync(s => s.Id == speciality.Id);
        entity.Name = speciality.Name;
        await _context.SaveChangesAsync();
    }
}
