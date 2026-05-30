using ClinicAPI.Models.Dtos.Patient;
using ClinicAPI.Services.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/patients")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAllPatients()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAllPatientsIncludingInactive()
        {
            var patients = await _patientService.GetAllIncludingInactiveAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
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

            await _patientService.UpdatePatient(patientData);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeactivatePatientById(int id)
        {
            if (!await _patientService.Exists(id))
            {
                return NotFound();
            }

            await _patientService.DeactivatePatient(id);
            return NoContent();
        }

        [HttpPatch("{id}/reactivate")]
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
