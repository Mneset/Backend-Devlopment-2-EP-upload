using ClinicAPI.Models.Dtos.Doctor;
using ClinicAPI.Services.Clinics;
using ClinicAPI.Services.Doctors;
using ClinicAPI.Services.Specialities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/doctors")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ISpecialityService _specialityService;
        private readonly IClinicService _clinicService;

        public DoctorController(
            IDoctorService doctorService,
            ISpecialityService specialityService,
            IClinicService clinicService)
        {
            _doctorService = doctorService;
            _specialityService = specialityService;
            _clinicService = clinicService;
        }

        /// <summary>
        /// Gets all active doctors. Public.
        /// </summary>
        /// <response code="200">Returns the list of active doctors.</response>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<DoctorResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        /// <summary>
        /// Gets all doctors including inactive ones. Admin only.
        /// </summary>
        /// <response code="200">Returns the list of all doctors.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpGet("all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<DoctorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetAllDoctorsIncludingInactive()
        {
            var doctors = await _doctorService.GetAllIncludingInactiveAsync();
            return Ok(doctors);
        }

        /// <summary>
        /// Searches for active doctors by first or last name. Public.
        /// </summary>
        /// <param name="name">The name fragment to match against first or last name (case-insensitive).</param>
        /// <response code="200">Returns the matching doctors.</response>
        /// <response code="400">The search term is empty.</response>
        [HttpGet("search")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<DoctorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> SearchDoctors([FromQuery] string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var doctors = await _doctorService.SearchByNameAsync(name);
            return Ok(doctors);
        }

        /// <summary>
        /// Gets an active doctor by id. Public.
        /// </summary>
        /// <param name="id">The id of the doctor to retrieve.</param>
        /// <response code="200">Returns the doctor with the specified id.</response>
        /// <response code="404">Doctor not found or not active.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DoctorResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorResponseDto>> GetDoctorById(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }

        /// <summary>
        /// Gets any doctor by id, including inactive ones. Admin only.
        /// </summary>
        /// <param name="id">The id of the doctor to retrieve.</param>
        /// <response code="200">Returns the doctor with the specified id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Doctor not found.</response>
        [HttpGet("all/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DoctorResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorResponseDto>> GetDoctorByIdIncludingInactive(int id)
        {
            var doctor = await _doctorService.GetByIdIncludingInactiveAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }

        /// <summary>
        /// Creates a new doctor. Admin only.
        /// </summary>
        /// <param name="doctorData">The doctor data to create. The referenced speciality and clinic must exist and the clinic must be active.</param>
        /// <response code="201">The doctor was created.</response>
        /// <response code="400">The request body is invalid, or the speciality or clinic does not exist or is not active.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DoctorResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DoctorResponseDto>> CreateDoctor([FromBody] CreateDoctorRequestDto doctorData)
        {
            if (!await _specialityService.Exists(doctorData.SpecialityId))
            {
                return BadRequest("Speciality does not exist.");
            }

            if (await _clinicService.GetByIdAsync(doctorData.ClinicId) == null)
            {
                return BadRequest("Clinic does not exist or is not active.");
            }

            var doctor = await _doctorService.AddDoctor(doctorData);
            return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
        }

        /// <summary>
        /// Updates an existing doctor. Admin only.
        /// </summary>
        /// <param name="id">The id from the route.</param>
        /// <param name="doctorData">The updated doctor data. The Id field must match the route id.</param>
        /// <response code="204">The doctor was updated.</response>
        /// <response code="400">The route id does not match the body id, or the speciality or clinic does not exist or is not active.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Doctor not found.</response>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDoctorById(int id, [FromBody] UpdateDoctorRequestDto doctorData)
        {
            if (id != doctorData.Id)
            {
                return BadRequest("Id in route does not match the Id in the request body");
            }

            if (!await _doctorService.Exists(doctorData.Id))
            {
                return NotFound();
            }

            if (!await _specialityService.Exists(doctorData.SpecialityId))
            {
                return BadRequest("Speciality does not exist.");
            }

            if (await _clinicService.GetByIdAsync(doctorData.ClinicId) == null)
            {
                return BadRequest("Clinic does not exist or is not active.");
            }

            await _doctorService.UpdateDoctor(doctorData);
            return NoContent();
        }

        /// <summary>
        /// Deactivates a doctor by id. Admin only.
        /// </summary>
        /// <param name="id">The id of the doctor to deactivate.</param>
        /// <response code="204">The doctor was deactivated.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Doctor not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeactivateDoctorById(int id)
        {
            if (!await _doctorService.Exists(id))
            {
                return NotFound();
            }

            await _doctorService.DeactivateDoctor(id);
            return NoContent();
        }

        /// <summary>
        /// Reactivates a previously deactivated doctor. Admin only.
        /// </summary>
        /// <param name="id">The id of the doctor to reactivate.</param>
        /// <response code="204">The doctor was reactivated.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Doctor not found.</response>
        [HttpPatch("{id}/reactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReactivateDoctorById(int id)
        {
            if (!await _doctorService.Exists(id))
            {
                return NotFound();
            }

            await _doctorService.ReactivateDoctor(id);
            return NoContent();
        }
    }
}
