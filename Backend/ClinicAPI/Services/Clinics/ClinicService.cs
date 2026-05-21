using ClinicAPI.Data;
using ClinicAPI.Models.Dtos.Clinic;
using ClinicAPI.Models.Entities;
using ClinicAPI.Services.Doctors;
using Microsoft.EntityFrameworkCore;

namespace ClinicAPI.Services.Clinics;

public class ClinicService : IClinicService
{

    private readonly ClinicDbContext _context;
    private readonly IDoctorService _doctorService;

        public ClinicService(ClinicDbContext context, IDoctorService doctorService)
        {
            _context = context;
            _doctorService = doctorService;
        }

    public async Task<bool> Exists(int id)
    {
        return await _context.Clinics.AnyAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<ClinicResponseDto>> GetAllAsync()
    {
        return await _context.Clinics
            .Where(c => c.IsActive)
            .Select(c => new ClinicResponseDto {Id = c.Id, Name = c.Name, IsActive = c.IsActive })
            .ToListAsync();
    }

    public async Task<IEnumerable<ClinicResponseDto>> GetAllIncludingInactiveAsync()
    {
        return await _context.Clinics
            .Select(c => new ClinicResponseDto {Id = c.Id, Name = c.Name, IsActive = c.IsActive })
            .ToListAsync();
    }

    public async Task<ClinicResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        if(entity == null) return null;
        return new ClinicResponseDto {Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive };
    }

    public async Task<ClinicResponseDto?> GetByIdIncludingInactiveAsync(int id)
    {
        var entity = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == id);
        if(entity == null) return null;
        return new ClinicResponseDto {Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive };
    }
    public async Task<ClinicResponseDto> AddClinic(CreateClinicRequestDto clinic)
    {
       var entity = new Clinic { Name = clinic.Name, IsActive = true };
       _context.Clinics.Add(entity);
       await _context.SaveChangesAsync();
       return new ClinicResponseDto { Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive };
    }

    public async Task DeactivateClinic(int id)
    {
        var clinic = await _context.Clinics
            .Include(c => c.Doctors)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clinic == null || !clinic.IsActive) return;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        foreach (var doctor in clinic.Doctors.ToList())
        {
            await _doctorService.DeactivateDoctor(doctor.Id);
        }

        clinic.IsActive = false;
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }

    public async Task ReactivateClinic(int id)
    {
        var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == id);

        if (clinic == null || clinic.IsActive) return;

        clinic.IsActive = true;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateClinic(UpdateClinicRequestDto clinic)
    {
        var entity = await _context.Clinics.FirstAsync(c => c.Id == clinic.Id);
        entity.Name = clinic.Name;
        await _context.SaveChangesAsync();
    }
}
