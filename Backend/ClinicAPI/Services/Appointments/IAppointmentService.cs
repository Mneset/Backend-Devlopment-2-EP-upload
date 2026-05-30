using ClinicAPI.Models.Dtos.Appointment;
using ClinicAPI.Models.Entities;
using ClinicAPI.Services.Shared;

namespace ClinicAPI.Services.Appointments;

public interface IAppointmentService : IBaseService<AppointmentResponseDto, int>
{
    public Task<bool> Exists(int id);
    public Task<IEnumerable<AppointmentResponseDto>> GetByStatusAsync(Status status);
    public Task<IEnumerable<AppointmentResponseDto>> GetByPatientAsync(int patientId);
    public Task<bool> HasConflict(int patientId, int doctorId, DateTime appointmentDate, int durationInMin, int? excludeAppointmentId = null);
    public Task<AppointmentResponseDto> BookAppointment(int patientId, BookAppointmentRequestDto request);
    public Task RescheduleAppointment(RescheduleAppointmentRequestDto request);
    public Task CancelAppointment(int id);
    public Task CompleteAppointment(int id);
}
