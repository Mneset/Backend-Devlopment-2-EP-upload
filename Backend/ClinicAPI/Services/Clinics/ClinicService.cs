using ClinicAPI.Data;
using ClinicAPI.Models.Dtos.Clinic;
using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAPI.Services.Clinics;

public class ClinicService : IClinicService
{

    private readonly ClinicDbContext _context;

        public ClinicService(ClinicDbContext context)
        {
            _context = context;
        }

    public async Task<bool> Exists(int id)
    {
        return await _context.Clinic.AnyAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<ClinicResponseDto>> GetAllAsync()
    {
        return await _context.Clinic
            .Select(c => new ClinicResponseDto {Id = c.Id, Name = c.Name })
            .ToListAsync();
    }

    public async Task<ClinicResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.Clinic.FirstOrDefaultAsync(c => c.Id == id);
        if(entity == null) return null;
        return new ClinicResponseDto {Id = entity.Id, Name = entity.Name };
    }
    public async Task<ClinicResponseDto> AddClinic(CreateClinicRequestDto clinic)
    {
       var entity = new Clinic { Name = clinic.Name };
       _context.Clinic.Add(entity);
       await _context.SaveChangesAsync();
       return new ClinicResponseDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task DeleteClinic(int id)
    {
       await _context.Clinic.Where(c => c.Id ==id ).ExecuteDeleteAsync();
    }

    public async Task UpdateClinic(int id, UpdateClinicRequestDto clinic)
    {
        var entity = await _context.Clinic.FirstAsync(c => c.Id == id);
        entity.Name = clinic.Name;
        await _context.SaveChangesAsync();
    }
}
