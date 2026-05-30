using ClinicAPI.Data;
using ClinicAPI.Models.Dtos.Appointment;
using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAPI.Services.Appointments;

public class AppointmentService : IAppointmentService
{
    private readonly ClinicDbContext _context;

    public AppointmentService(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Exists(int id)
    {
        return await _context.Appointments.AnyAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetAllAsync()
    {
        return await _context.Appointments
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                Status = a.Status,
                AppointmentDate = a.AppointmentDate,
                DurationInMin = a.DurationInMin,
                PatientId = a.PatientId,
                PatientFirstName = a.Patient.FirstName,
                PatientLastName = a.Patient.LastName,
                DoctorId = a.DoctorId,
                DoctorFirstName = a.Doctor.FirstName,
                DoctorLastName = a.Doctor.LastName,
                ClinicId = a.ClinicId,
                ClinicName = a.Clinic.Name,
                AppointmentCategoryId = a.AppointmentCategoryId,
                AppointmentCategoryName = a.AppointmentCategory.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetByStatusAsync(Status status)
    {
        return await _context.Appointments
            .Where(a => a.Status == status)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                Status = a.Status,
                AppointmentDate = a.AppointmentDate,
                DurationInMin = a.DurationInMin,
                PatientId = a.PatientId,
                PatientFirstName = a.Patient.FirstName,
                PatientLastName = a.Patient.LastName,
                DoctorId = a.DoctorId,
                DoctorFirstName = a.Doctor.FirstName,
                DoctorLastName = a.Doctor.LastName,
                ClinicId = a.ClinicId,
                ClinicName = a.Clinic.Name,
                AppointmentCategoryId = a.AppointmentCategoryId,
                AppointmentCategoryName = a.AppointmentCategory.Name
            })
            .ToListAsync();
    }

    public async Task<AppointmentResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .Where(a => a.Id == id)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                Status = a.Status,
                AppointmentDate = a.AppointmentDate,
                DurationInMin = a.DurationInMin,
                PatientId = a.PatientId,
                PatientFirstName = a.Patient.FirstName,
                PatientLastName = a.Patient.LastName,
                DoctorId = a.DoctorId,
                DoctorFirstName = a.Doctor.FirstName,
                DoctorLastName = a.Doctor.LastName,
                ClinicId = a.ClinicId,
                ClinicName = a.Clinic.Name,
                AppointmentCategoryId = a.AppointmentCategoryId,
                AppointmentCategoryName = a.AppointmentCategory.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetByPatientAsync(int patientId)
    {
        return await _context.Appointments
            .Where(a => a.PatientId == patientId)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                Status = a.Status,
                AppointmentDate = a.AppointmentDate,
                DurationInMin = a.DurationInMin,
                PatientId = a.PatientId,
                PatientFirstName = a.Patient.FirstName,
                PatientLastName = a.Patient.LastName,
                DoctorId = a.DoctorId,
                DoctorFirstName = a.Doctor.FirstName,
                DoctorLastName = a.Doctor.LastName,
                ClinicId = a.ClinicId,
                ClinicName = a.Clinic.Name,
                AppointmentCategoryId = a.AppointmentCategoryId,
                AppointmentCategoryName = a.AppointmentCategory.Name
            })
            .ToListAsync();
    }

    public async Task<bool> HasConflict(int patientId, int doctorId, DateTime appointmentDate, int durationInMin, int? excludeAppointmentId = null)
    {
        var proposedEnd = appointmentDate.AddMinutes(durationInMin);

        return await _context.Appointments.AnyAsync(a =>
            a.Status == Status.Scheduled &&
            (excludeAppointmentId == null || a.Id != excludeAppointmentId) &&
            (a.PatientId == patientId || a.DoctorId == doctorId) &&
            a.AppointmentDate < proposedEnd &&
            a.AppointmentDate.AddMinutes(a.DurationInMin) > appointmentDate);
    }

    public async Task<AppointmentResponseDto> BookAppointment(int patientId, BookAppointmentRequestDto request)
    {
        var entity = new Appointment
        {
            PatientId = patientId,
            DoctorId = request.DoctorId,
            ClinicId = request.ClinicId,
            AppointmentCategoryId = request.AppointmentCategoryId,
            AppointmentDate = request.AppointmentDate,
            DurationInMin = request.DurationInMin,
            Status = Status.Scheduled
        };

        _context.Appointments.Add(entity);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task RescheduleAppointment(RescheduleAppointmentRequestDto request)
    {
        var entity = await _context.Appointments.FirstAsync(a => a.Id == request.Id);
        entity.AppointmentDate = request.AppointmentDate;
        entity.DurationInMin = request.DurationInMin;
        await _context.SaveChangesAsync();
    }

    public async Task CancelAppointment(int id)
    {
        var entity = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (entity == null || entity.Status != Status.Scheduled) return;
        entity.Status = Status.Cancelled;
        await _context.SaveChangesAsync();
    }

    public async Task CompleteAppointment(int id)
    {
        var entity = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (entity == null || entity.Status != Status.Scheduled) return;
        entity.Status = Status.Completed;
        await _context.SaveChangesAsync();
    }
}
