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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAllPatients()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAllPatientsIncludingInactive()
        {
            var patients = await _patientService.GetAllIncludingInactiveAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PatientResponseDto>> GetPatientById(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet("all/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PatientResponseDto>> GetPatientByIdIncludingInactive(int id)
        {
            var patient = await _patientService.GetByIdIncludingInactiveAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePatientById(int id, [FromBody] UpdatePatientRequestDto patientData)
        {
            if (id != patientData.Id)
            {
                return BadRequest("Id in route does not match the Id in the request body");
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

        [HttpPut("me")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateOwnProfileRequestDto request)
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

            if (await _authService.IsEmailTakenByAnotherPatient(request.Email, patientId.Value))
            {
                return Conflict("Email is already taken.");
            }

            await _patientService.UpdateOwnProfile(patientId.Value, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivatePatientById(int id)
        {
            if (!await _patientService.Exists(id))
            {
                return NotFound();
            }

            await _patientService.DeactivatePatient(id);
            return NoContent();
        }

        [HttpDelete("me")]
        [Authorize(Roles = "Patient")]
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

        [HttpPatch("{id}/reactivate")]
        [Authorize(Roles = "Admin")]
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
