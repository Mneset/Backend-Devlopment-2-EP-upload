using ClinicAPI.Authorization;
using ClinicAPI.Models.Dtos.Appointment;
using ClinicAPI.Models.Dtos.Patient;
using ClinicAPI.Models.Entities;
using ClinicAPI.Services.AppointmentCategories;
using ClinicAPI.Services.Appointments;
using ClinicAPI.Services.Clinics;
using ClinicAPI.Services.Doctors;
using ClinicAPI.Services.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/appointments")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IClinicService _clinicService;
        private readonly IAppointmentCategoryService _appointmentCategoryService;

        public AppointmentController(
            IAppointmentService appointmentService,
            IPatientService patientService,
            IDoctorService doctorService,
            IClinicService clinicService,
            IAppointmentCategoryService appointmentCategoryService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
            _clinicService = clinicService;
            _appointmentCategoryService = appointmentCategoryService;
        }

        private async Task<bool> CallerIsStillActivePatientAsync()
        {
            var patientId = User.GetPatientId();
            if (patientId == null)
            {
                return true;
            }
            return await _patientService.GetByIdAsync(patientId.Value) != null;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAllAppointments([FromQuery] Status? status = null)
        {
            var appointments = status.HasValue
                ? await _appointmentService.GetByStatusAsync(status.Value)
                : await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("mine")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetMyAppointments()
        {
            if (!await CallerIsStillActivePatientAsync()) return Forbid();

            var patientId = User.GetPatientId()!.Value;

            var appointments = await _appointmentService.GetByPatientAsync(patientId);
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentResponseDto>> GetAppointmentById(int id)
        {
            if (!await CallerIsStillActivePatientAsync()) return Forbid();

            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            if (!User.IsAdmin() && User.GetPatientId() != appointment.PatientId)
            {
                return Forbid();
            }

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentResponseDto>> BookAppointment([FromBody] BookAppointmentRequestDto request)
        {
            var patientId = User.GetPatientId();
            if (patientId == null)
            {
                return Forbid();
            }

            if (await _patientService.GetByIdAsync(patientId.Value) == null)
            {
                return BadRequest("Patient does not exist or is not active.");
            }

            var doctor = await _doctorService.GetByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                return BadRequest("Doctor does not exist or is not active.");
            }

            if (doctor.ClinicId != request.ClinicId)
            {
                return BadRequest("Doctor does not work at the chosen clinic.");
            }

            if (await _clinicService.GetByIdAsync(request.ClinicId) == null)
            {
                return BadRequest("Clinic does not exist or is not active.");
            }

            if (!await _appointmentCategoryService.Exists(request.AppointmentCategoryId))
            {
                return BadRequest("Appointment category does not exist.");
            }

            if (await _appointmentService.HasConflict(patientId.Value, request.DoctorId, request.AppointmentDate, request.DurationInMin))
            {
                return Conflict("Appointment overlaps with an existing scheduled appointment.");
            }

            var appointment = await _appointmentService.BookAppointment(patientId.Value, request);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        [HttpPost("guest")]
        [AllowAnonymous]
        public async Task<ActionResult<AppointmentResponseDto>> BookGuestAppointment([FromBody] BookGuestAppointmentRequestDto request)
        {
            if (request.DateOfBirth > DateTime.UtcNow)
            {
                return BadRequest("Date of birth cannot be in the future.");
            }

            var doctor = await _doctorService.GetByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                return BadRequest("Doctor does not exist or is not active.");
            }

            if (doctor.ClinicId != request.ClinicId)
            {
                return BadRequest("Doctor does not work at the chosen clinic.");
            }

            if (await _clinicService.GetByIdAsync(request.ClinicId) == null)
            {
                return BadRequest("Clinic does not exist or is not active.");
            }

            if (!await _appointmentCategoryService.Exists(request.AppointmentCategoryId))
            {
                return BadRequest("Appointment category does not exist.");
            }

            var guest = await _patientService.CreateGuestPatient(new CreateGuestPatientRequestDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth
            });

            var bookRequest = new BookAppointmentRequestDto
            {
                DoctorId = request.DoctorId,
                ClinicId = request.ClinicId,
                AppointmentCategoryId = request.AppointmentCategoryId,
                AppointmentDate = request.AppointmentDate,
                DurationInMin = request.DurationInMin
            };

            var appointment = await _appointmentService.BookAppointment(guest.Id, bookRequest);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        [HttpPatch("{id}/reschedule")]
        public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleAppointmentRequestDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("Id in route does not match the Id in the request body");
            }

            if (!await CallerIsStillActivePatientAsync()) return Forbid();

            var existing = await _appointmentService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (!User.IsAdmin() && User.GetPatientId() != existing.PatientId)
            {
                return Forbid();
            }

            if (existing.Status != Status.Scheduled)
            {
                return Conflict("Only scheduled appointments can be rescheduled.");
            }

            if (await _appointmentService.HasConflict(existing.PatientId, existing.DoctorId, request.AppointmentDate, request.DurationInMin, excludeAppointmentId: id))
            {
                return Conflict("New appointment time overlaps with an existing scheduled appointment.");
            }

            await _appointmentService.RescheduleAppointment(request);
            return NoContent();
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            if (!await CallerIsStillActivePatientAsync()) return Forbid();

            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (!User.IsAdmin() && User.GetPatientId() != appointment.PatientId)
            {
                return Forbid();
            }

            await _appointmentService.CancelAppointment(id);
            return NoContent();
        }

        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            if (!await _appointmentService.Exists(id))
            {
                return NotFound();
            }

            await _appointmentService.CompleteAppointment(id);
            return NoContent();
        }
    }
}
