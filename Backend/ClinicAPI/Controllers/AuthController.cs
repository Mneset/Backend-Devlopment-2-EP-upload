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

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="request">The login credentials.</param>
        /// <response code="200">Authentication succeeded. Returns a JWT token.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">The email or password is incorrect.</response>
        [HttpPost("login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.Login(request);

            if (result == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Registers a new patient with a login account.
        /// </summary>
        /// <param name="request">The registration details including credentials and personal information.</param>
        /// <response code="201">The patient was created.</response>
        /// <response code="400">The request body is invalid or the date of birth is in the future.</response>
        /// <response code="409">A patient with the given email already exists.</response>
        [HttpPost("register/patient")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

        /// <summary>
        /// Creates a guest patient record without a login account. Used by the guest booking flow.
        /// </summary>
        /// <param name="request">The guest's personal information.</param>
        /// <response code="201">The guest patient was created.</response>
        /// <response code="400">The request body is invalid.</response>
        [HttpPost("register/guest")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientResponseDto>> CreateGuestPatient([FromBody] CreateGuestPatientRequestDto request)
        {
            var patient = await _patientService.CreateGuestPatient(request);
            return CreatedAtAction("GetPatientById", "Patient", new { id = patient.Id }, patient);
        }
    }
}
