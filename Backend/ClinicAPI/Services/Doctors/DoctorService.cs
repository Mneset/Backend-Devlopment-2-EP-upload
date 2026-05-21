using ClinicAPI.Data;
using ClinicAPI.Models.Dtos.Doctor;
using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAPI.Services.Doctors;

public class DoctorService : IDoctorService
{
    private readonly ClinicDbContext _context;

    public DoctorService(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Exists(int id)
    {
        return await _context.Doctors.AnyAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<DoctorResponseDto>> GetAllAsync()
    {
        return await _context.Doctors
            .Where(d => d.IsActive)
            .Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Gender = d.Gender,
                IsActive = d.IsActive,
                SpecialityId = d.SpecialityId,
                SpecialityName = d.Speciality.Name,
                ClinicId = d.ClinicId,
                ClinicName = d.Clinic.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<DoctorResponseDto>> GetAllIncludingInactiveAsync()
    {
        return await _context.Doctors
            .Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Gender = d.Gender,
                IsActive = d.IsActive,
                SpecialityId = d.SpecialityId,
                SpecialityName = d.Speciality.Name,
                ClinicId = d.ClinicId,
                ClinicName = d.Clinic.Name
            })
            .ToListAsync();
    }

    public async Task<DoctorResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Doctors
            .Where(d => d.Id == id && d.IsActive)
            .Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Gender = d.Gender,
                IsActive = d.IsActive,
                SpecialityId = d.SpecialityId,
                SpecialityName = d.Speciality.Name,
                ClinicId = d.ClinicId,
                ClinicName = d.Clinic.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DoctorResponseDto?> GetByIdIncludingInactiveAsync(int id)
    {
        return await _context.Doctors
            .Where(d => d.Id == id)
            .Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Gender = d.Gender,
                IsActive = d.IsActive,
                SpecialityId = d.SpecialityId,
                SpecialityName = d.Speciality.Name,
                ClinicId = d.ClinicId,
                ClinicName = d.Clinic.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<DoctorResponseDto>> SearchByNameAsync(string name)
    {
        return await _context.Doctors
            .Where(d => d.IsActive && (d.FirstName.Contains(name) || d.LastName.Contains(name)))
            .Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Gender = d.Gender,
                IsActive = d.IsActive,
                SpecialityId = d.SpecialityId,
                SpecialityName = d.Speciality.Name,
                ClinicId = d.ClinicId,
                ClinicName = d.Clinic.Name
            })
            .ToListAsync();
    }

    public async Task<DoctorResponseDto> AddDoctor(CreateDoctorRequestDto request)
    {
        var entity = new Doctor
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            SpecialityId = request.SpecialityId,
            ClinicId = request.ClinicId,
            IsActive = true
        };

        _context.Doctors.Add(entity);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task UpdateDoctor(UpdateDoctorRequestDto request)
    {
        var entity = await _context.Doctors.FirstAsync(d => d.Id == request.Id);
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Gender = request.Gender;
        entity.SpecialityId = request.SpecialityId;
        entity.ClinicId = request.ClinicId;
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateDoctor(int id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Appointments)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null || !doctor.IsActive) return;

        doctor.IsActive = false;

        foreach (var appointment in doctor.Appointments.Where(a => a.Status == Status.Scheduled))
        {
            appointment.Status = Status.Cancelled;
        }

        await _context.SaveChangesAsync();
    }

    public async Task ReactivateDoctor(int id)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null || doctor.IsActive) return;

        doctor.IsActive = true;
        await _context.SaveChangesAsync();
    }
}
