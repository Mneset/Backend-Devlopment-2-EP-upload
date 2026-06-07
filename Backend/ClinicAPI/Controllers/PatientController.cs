using ClinicAPI.Authorization;
using ClinicAPI.Models.Dtos.Patient;
using ClinicAPI.Services.Auth;
using ClinicAPI.Services.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/patients")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IAuthService _authService;

        public PatientController(IPatientService patientService, IAuthService authService)
        {
            _patientService = patientService;
            _authService = authService;
        }

        /// <summary>
        /// Gets all active patients. Admin only.
        /// </summary>
        /// <response code="200">Returns the list of active patients.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<PatientResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAllPatients()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        /// <summary>
        /// Gets all patients including inactive ones. Admin only.
        /// </summary>
        /// <response code="200">Returns the list of all patients.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<PatientResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAllPatientsIncludingInactive()
        {
            var patients = await _patientService.GetAllIncludingInactiveAsync();
            return Ok(patients);
        }

        /// <summary>
        /// Gets an active patient by id. Admin only.
        /// </summary>
        /// <param name="id">The id of the patient to retrieve.</param>
        /// <response code="200">Returns the patient with the specified id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Patient not found or not active.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientResponseDto>> GetPatientById(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        /// <summary>
        /// Gets any patient by id, including inactive ones. Admin only.
        /// </summary>
        /// <param name="id">The id of the patient to retrieve.</param>
        /// <response code="200">Returns the patient with the specified id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Patient not found.</response>
        [HttpGet("all/{id}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientResponseDto>> GetPatientByIdIncludingInactive(int id)
        {
            var patient = await _patientService.GetByIdIncludingInactiveAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        /// <summary>
        /// Updates an existing patient. Admin only.
        /// </summary>
        /// <param name="id">The id from the route.</param>
        /// <param name="patientData">The updated patient data. The Id field must match the route id.</param>
        /// <response code="204">The patient was updated.</response>
        /// <response code="400">The route id does not match the body id or the date of birth is in the future.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Patient not found.</response>
        /// <response code="409">The email is already taken by another patient.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePatientById(int id, [FromBody] UpdatePatientRequestDto patientData)
        {
            if (id != patientData.Id)
            {
                return BadRequest("Id in route does not match the Id in the request body");
            }

            if (patientData.DateOfBirth > DateTime.UtcNow)
            {
                return BadRequest("Date of birth cannot be in the future.");
            }

            if (!await _patientService.Exists(patientData.Id))
            {
                return NotFound();
            }

            if (await _authService.IsEmailTakenByAnotherPatient(patientData.Email, patientData.Id))
            {
                return Conflict("Email is already taken.");
            }

            await _patientService.UpdatePatient(patientData);
            return NoContent();
        }

        /// <summary>
        /// Gets the profile of the currently logged-in patient. Patient only.
        /// </summary>
        /// <response code="200">Returns the caller's profile.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not a patient or the patient has been deactivated.</response>
        [HttpGet("me")]
        [Authorize(Roles = "Patient")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MyProfileResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MyProfileResponseDto>> GetMyProfile()
        {
            var patientId = User.GetPatientId();
            if (patientId == null)
            {
                return Forbid();
            }

            if (await _patientService.GetByIdAsync(patientId.Value) == null)
            {
                return Forbid();
            }

            var profile = await _patientService.GetMyProfile(patientId.Value);
            if (profile == null)
            {
                return Forbid();
            }

            return Ok(profile);
        }

        /// <summary>
        /// Updates the profile of the currently logged-in patient. Patient only.
        /// </summary>
        /// <param name="request">The updated profile data.</param>
        /// <response code="204">The profile was updated.</response>
        /// <response code="400">The date of birth is in the future.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not a patient or has been deactivated.</response>
        /// <response code="409">The email is already taken by another patient.</response>
        [HttpPut("me")]
        [Authorize(Roles = "Patient")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateOwnProfileRequestDto request)
        {
            if (request.DateOfBirth > DateTime.UtcNow)
            {
                return BadRequest("Date of birth cannot be in the future.");
            }

            var patientId = User.GetPatientId();
            if (patientId == null)
            {
                return Forbid();
            }

            if (await _patientService.GetByIdAsync(patientId.Value) == null)
            {
                return Forbid();
            }

            if (await _authService.IsEmailTakenByAnotherPatient(request.Email, patientId.Value))
            {
                return Conflict("Email is already taken.");
            }

            await _patientService.UpdateOwnProfile(patientId.Value, request);
            return NoContent();
        }

        /// <summary>
        /// Deactivates a patient by id. Admin only. Cancels any scheduled appointments for the patient.
        /// </summary>
        /// <param name="id">The id of the patient to deactivate.</param>
        /// <response code="204">The patient was deactivated.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Patient not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeactivatePatientById(int id)
        {
            if (!await _patientService.Exists(id))
            {
                return NotFound();
            }

            await _patientService.DeactivatePatient(id);
            return NoContent();
        }

        /// <summary>
        /// Deactivates the currently logged-in patient's own account. Patient only.
        /// </summary>
        /// <response code="204">The account was deactivated.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not a patient or is already deactivated.</response>
        [HttpDelete("me")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var patientId = User.GetPatientId();
            if (patientId == null)
            {
                return Forbid();
            }

            if (await _patientService.GetByIdAsync(patientId.Value) == null)
            {
                return Forbid();
            }

            await _patientService.DeactivatePatient(patientId.Value);
            return NoContent();
        }

        /// <summary>
        /// Reactivates a previously deactivated patient. Admin only.
        /// </summary>
        /// <param name="id">The id of the patient to reactivate.</param>
        /// <response code="204">The patient was reactivated.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Patient not found.</response>
        [HttpPatch("{id}/reactivate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReactivatePatientById(int id)
        {
            if (!await _patientService.Exists(id))
            {
                return NotFound();
            }

            await _patientService.ReactivatePatient(id);
            return NoContent();
        }
    }
}
