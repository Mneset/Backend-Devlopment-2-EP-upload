using ClinicAPI.Models.Dtos.Auth;
using ClinicAPI.Models.Dtos.Patient;
using ClinicAPI.Services.Auth;
using ClinicAPI.Services.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IPatientService _patientService;

        public AuthController(IAuthService authService, IPatientService patientService)
        {
            _authService = authService;
            _patientService = patientService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.Login(request);

            if (result == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(result);
        }

        [HttpPost("register/patient")]
        public async Task<ActionResult<PatientResponseDto>> Register([FromBody] RegisterPatientRequestDto request)
        {
            if (request.DateOfBirth > DateTime.UtcNow)
            {
                return BadRequest("Date of birth cannot be in the future.");
            }

            if (await _authService.IsEmailTaken(request.Email))
            {
                return Conflict("Email is already taken.");
            }

            var patient = await _patientService.RegisterPatient(request);
            return CreatedAtAction("GetPatientById", "Patient", new { id = patient.Id }, patient);
        }

        [HttpPost("register/guest")]
        public async Task<ActionResult<PatientResponseDto>> CreateGuestPatient([FromBody] CreateGuestPatientRequestDto request)
        {
            var patient = await _patientService.CreateGuestPatient(request);
            return CreatedAtAction("GetPatientById", "Patient", new { id = patient.Id }, patient);
        }
    }
}
