using ClinicAPI.Data;
using ClinicAPI.Models.Dtos.Patient;
using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAPI.Services.Patients;

public class PatientService : IPatientService
{
    private readonly ClinicDbContext _context;

    public PatientService(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Exists(int id)
    {
        return await _context.Patients.AnyAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
    {
        return await _context.Patients
            .Where(p => p.IsActive)
            .Select(p => new PatientResponseDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                IsActive = p.IsActive,
                IsRegistered = p.IsRegistered,
                Religion = p.Religion,
                SocialSecurityNumber = p.SocialSecurityNumber,
                TaxNumber = p.TaxNumber,
                DriverLicenseNumber = p.DriverLicenseNumber,
                MedicalInsuranceNumber = p.MedicalInsuranceNumber
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientResponseDto>> GetAllIncludingInactiveAsync()
    {
        return await _context.Patients
            .Select(p => new PatientResponseDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                IsActive = p.IsActive,
                IsRegistered = p.IsRegistered,
                Religion = p.Religion,
                SocialSecurityNumber = p.SocialSecurityNumber,
                TaxNumber = p.TaxNumber,
                DriverLicenseNumber = p.DriverLicenseNumber,
                MedicalInsuranceNumber = p.MedicalInsuranceNumber
            })
            .ToListAsync();
    }

    public async Task<PatientResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Patients
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new PatientResponseDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                IsActive = p.IsActive,
                IsRegistered = p.IsRegistered,
                Religion = p.Religion,
                SocialSecurityNumber = p.SocialSecurityNumber,
                TaxNumber = p.TaxNumber,
                DriverLicenseNumber = p.DriverLicenseNumber,
                MedicalInsuranceNumber = p.MedicalInsuranceNumber
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PatientResponseDto?> GetByIdIncludingInactiveAsync(int id)
    {
        return await _context.Patients
            .Where(p => p.Id == id)
            .Select(p => new PatientResponseDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                IsActive = p.IsActive,
                IsRegistered = p.IsRegistered,
                Religion = p.Religion,
                SocialSecurityNumber = p.SocialSecurityNumber,
                TaxNumber = p.TaxNumber,
                DriverLicenseNumber = p.DriverLicenseNumber,
                MedicalInsuranceNumber = p.MedicalInsuranceNumber
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PatientResponseDto> RegisterPatient(RegisterPatientRequestDto request)
    {
        var entity = new Patient
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            Religion = request.Religion,
            SocialSecurityNumber = request.SocialSecurityNumber,
            TaxNumber = request.TaxNumber,
            DriverLicenseNumber = request.DriverLicenseNumber,
            MedicalInsuranceNumber = request.MedicalInsuranceNumber,
            IsActive = true,
            IsRegistered = true
        };

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Patient,
            Patient = entity
        };

        _context.Patients.Add(entity);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new PatientResponseDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Gender = entity.Gender,
            DateOfBirth = entity.DateOfBirth,
            IsActive = entity.IsActive,
            IsRegistered = entity.IsRegistered,
            Religion = entity.Religion,
            SocialSecurityNumber = entity.SocialSecurityNumber,
            TaxNumber = entity.TaxNumber,
            DriverLicenseNumber = entity.DriverLicenseNumber,
            MedicalInsuranceNumber = entity.MedicalInsuranceNumber
        };
    }

    public async Task<PatientResponseDto> CreateGuestPatient(CreateGuestPatientRequestDto request)
    {
        var entity = new Patient
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            IsActive = true,
            IsRegistered = false
        };

        _context.Patients.Add(entity);
        await _context.SaveChangesAsync();

        return new PatientResponseDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Gender = entity.Gender,
            DateOfBirth = entity.DateOfBirth,
            IsActive = entity.IsActive,
            IsRegistered = entity.IsRegistered,
            Religion = entity.Religion,
            SocialSecurityNumber = entity.SocialSecurityNumber,
            TaxNumber = entity.TaxNumber,
            DriverLicenseNumber = entity.DriverLicenseNumber,
            MedicalInsuranceNumber = entity.MedicalInsuranceNumber
        };
    }

    public async Task<MyProfileResponseDto?> GetMyProfile(int patientId)
    {
        return await _context.Users
            .Where(u => u.PatientId == patientId && u.Patient != null && u.Patient.IsActive)
            .Select(u => new MyProfileResponseDto
            {
                Id = u.Patient!.Id,
                Email = u.Email,
                FirstName = u.Patient.FirstName,
                LastName = u.Patient.LastName,
                Gender = u.Patient.Gender,
                DateOfBirth = u.Patient.DateOfBirth,
                Religion = u.Patient.Religion,
                SocialSecurityNumber = u.Patient.SocialSecurityNumber,
                TaxNumber = u.Patient.TaxNumber,
                DriverLicenseNumber = u.Patient.DriverLicenseNumber,
                MedicalInsuranceNumber = u.Patient.MedicalInsuranceNumber
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdatePatient(UpdatePatientRequestDto request)
    {
        var entity = await _context.Patients.FirstAsync(p => p.Id == request.Id);

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Gender = request.Gender;
        entity.DateOfBirth = request.DateOfBirth;

        if (entity.IsRegistered)
        {
            entity.Religion = request.Religion;
            entity.SocialSecurityNumber = request.SocialSecurityNumber;
            entity.TaxNumber = request.TaxNumber;
            entity.DriverLicenseNumber = request.DriverLicenseNumber;
            entity.MedicalInsuranceNumber = request.MedicalInsuranceNumber;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.PatientId == request.Id);
        if (user != null)
        {
            user.Email = request.Email;
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateOwnProfile(int patientId, UpdateOwnProfileRequestDto request)
    {
        var entity = await _context.Patients.FirstAsync(p => p.Id == patientId);

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Gender = request.Gender;
        entity.DateOfBirth = request.DateOfBirth;

        if (entity.IsRegistered)
        {
            entity.Religion = request.Religion;
        }

        var user = await _context.Users.FirstAsync(u => u.PatientId == patientId);
        user.Email = request.Email;

        await _context.SaveChangesAsync();
    }

    public async Task DeactivatePatient(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null || !patient.IsActive) return;

        patient.IsActive = false;

        foreach (var appointment in patient.Appointments.Where(a => a.Status == Status.Scheduled))
        {
            appointment.Status = Status.Cancelled;
        }

        await _context.SaveChangesAsync();
    }

    public async Task ReactivatePatient(int id)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null || patient.IsActive) return;

        patient.IsActive = true;
        await _context.SaveChangesAsync();
    }
}
