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

        /// <summary>
        /// Gets all appointments, optionally filtered by status. Admin only.
        /// </summary>
        /// <param name="status">Optional status filter (Scheduled, Completed or Cancelled).</param>
        /// <response code="200">Returns the list of appointments.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAllAppointments([FromQuery] Status? status = null)
        {
            var appointments = status.HasValue
                ? await _appointmentService.GetByStatusAsync(status.Value)
                : await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        /// <summary>
        /// Gets all appointments for the currently logged-in patient. Patient only.
        /// </summary>
        /// <response code="200">Returns the caller's appointments.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not a patient or has been deactivated.</response>
        [HttpGet("mine")]
        [Authorize(Roles = "Patient")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetMyAppointments()
        {
            if (!await CallerIsStillActivePatientAsync()) return Forbid();

            var patientId = User.GetPatientId()!.Value;

            var appointments = await _appointmentService.GetByPatientAsync(patientId);
            return Ok(appointments);
        }

        /// <summary>
        /// Gets an appointment by id. Patients may only view their own; admins may view any.
        /// </summary>
        /// <param name="id">The id of the appointment to retrieve.</param>
        /// <response code="200">Returns the appointment with the specified id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Patient is trying to view another patient's appointment or has been deactivated.</response>
        /// <response code="404">Appointment not found.</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AppointmentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Books an appointment for the currently logged-in patient.
        /// </summary>
        /// <param name="request">The appointment details: doctor, clinic, category, date and duration.</param>
        /// <response code="201">The appointment was booked.</response>
        /// <response code="400">The request body is invalid, or the referenced doctor, clinic or category does not exist or is not active, or the doctor does not work at the chosen clinic.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not a patient.</response>
        /// <response code="409">The chosen time overlaps with an existing scheduled appointment.</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AppointmentResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

        /// <summary>
        /// Books an appointment for a new guest patient. Public.
        /// Creates the guest record and the appointment in a single call.
        /// </summary>
        /// <param name="request">Guest information plus appointment details.</param>
        /// <response code="201">The guest patient was created and the appointment was booked.</response>
        /// <response code="400">The request body is invalid, the date of birth is in the future, or the referenced doctor, clinic or category does not exist or is not active.</response>
        [HttpPost("guest")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AppointmentResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Reschedules an existing scheduled appointment. Patient may reschedule their own; admin may reschedule any.
        /// </summary>
        /// <param name="id">The id from the route.</param>
        /// <param name="request">The new date and duration. The Id field must match the route id.</param>
        /// <response code="204">The appointment was rescheduled.</response>
        /// <response code="400">The route id does not match the body id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Patient is trying to reschedule another patient's appointment.</response>
        /// <response code="404">Appointment not found.</response>
        /// <response code="409">The appointment is not in the Scheduled state, or the new time overlaps with another appointment.</response>
        [HttpPatch("{id}/reschedule")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

        /// <summary>
        /// Cancels an existing appointment. Patient may cancel their own; admin may cancel any.
        /// </summary>
        /// <param name="id">The id of the appointment to cancel.</param>
        /// <response code="204">The appointment was cancelled.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Patient is trying to cancel another patient's appointment.</response>
        /// <response code="404">Appointment not found.</response>
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Marks an appointment as completed. Admin only.
        /// </summary>
        /// <param name="id">The id of the appointment to complete.</param>
        /// <response code="204">The appointment was marked as completed.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Appointment not found.</response>
        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
