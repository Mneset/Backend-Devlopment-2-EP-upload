using ClinicAPI.Data;
using ClinicAPI.Models.Dtos.Speciality;
using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAPI.Services.Specialtities;

public class SpecialityService : ISpecialityService
{
    private readonly ClinicDbContext _context;

        public SpecialityService(ClinicDbContext context)
        {
            _context = context;
        }

    public async Task<bool> Exists(int id)
    {
        return await _context.Specialities.AnyAsync(c => c.Id == id);
        
    }

    public async Task<IEnumerable<SpecialityResponseDto>> GetAllAsync()
    {
        return await _context.Specialities
            .Select(s => new SpecialityResponseDto {Name = s.Name })
            .ToListAsync();
    }

    public async Task<SpecialityResponseDto> GetByIdAsync(int id)
    {
        var speciality = await _context.Specialities.FirstAsync(c => c.Id == id);
        return new SpecialityResponseDto { Name = speciality.Name };
    }
    public async Task<SpecialityResponseDto> AddSpeciality(CreateSpecialityRequestDto speciality)
    {
        var entity = new Speciality { Name = speciality.Name };
       _context.Specialities.Add(entity);
       await _context.SaveChangesAsync();
       return new SpecialityResponseDto { Name = entity.Name };
    }

    public async Task DeleteSpeciality(int id)
    {
        await _context.Specialities.Where(s => s.Id == id ).ExecuteDeleteAsync();
    }

    public async Task<SpecialityResponseDto> UpdateSpeciality(int id, UpdateSpecialityRequestDto speciality)
    {
        var entity = await _context.Specialities.FirstAsync(s => s.Id == id);
        entity.Name = speciality.Name;
        await _context.SaveChangesAsync();
        return new SpecialityResponseDto { Name = entity.Name };
    }
}
