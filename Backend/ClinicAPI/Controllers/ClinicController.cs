using ClinicAPI.Models.Dtos.Clinic;
using ClinicAPI.Services.Clinics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/clinics")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ClinicController : ControllerBase
    {
        private readonly IClinicService _clinicService;

        public ClinicController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        /// <summary>
        /// Gets all active clinics. Public.
        /// </summary>
        /// <response code="200">Returns the list of active clinics.</response>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<ClinicResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClinicResponseDto>>> GetAllClinics()
        {
            var clinics = await _clinicService.GetAllAsync();
            return Ok(clinics);
        }

        /// <summary>
        /// Gets all clinics including inactive ones. Admin only.
        /// </summary>
        /// <response code="200">Returns the list of all clinics.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpGet("all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<ClinicResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ClinicResponseDto>>> GetAllClinicsIncludingInactive()
        {
            var clinics = await _clinicService.GetAllIncludingInactiveAsync();
            return Ok(clinics);
        }

        /// <summary>
        /// Gets an active clinic by its id. Public.
        /// </summary>
        /// <param name="id">The id of the clinic to retrieve.</param>
        /// <response code="200">Returns the clinic with the specified id.</response>
        /// <response code="404">Clinic not found or not active.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ClinicResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClinicResponseDto>> GetClinicById(int id)
        {
            var clinic = await _clinicService.GetByIdAsync(id);

            if (clinic == null)
            {
                return NotFound();
            }

            return Ok(clinic);
        }

        /// <summary>
        /// Gets any clinic by its id, including inactive ones. Admin only.
        /// </summary>
        /// <param name="id">The id of the clinic to retrieve.</param>
        /// <response code="200">Returns the clinic with the specified id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Clinic not found.</response>
        [HttpGet("all/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ClinicResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClinicResponseDto>> GetClinicByIdIncludingInactive(int id)
        {
            var clinic = await _clinicService.GetByIdIncludingInactiveAsync(id);

            if (clinic == null)
            {
                return NotFound();
            }

            return Ok(clinic);
        }

        /// <summary>
        /// Creates a new clinic. Admin only.
        /// </summary>
        /// <param name="clinicData">The clinic data to create.</param>
        /// <response code="201">The clinic was created.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ClinicResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClinicResponseDto>> CreateClinic([FromBody] CreateClinicRequestDto clinicData)
        {
            var clinic = await _clinicService.AddClinic(clinicData);
            return CreatedAtAction(nameof(GetClinicById), new { id = clinic.Id }, clinic);
        }

        /// <summary>
        /// Deactivates a clinic by its id. Admin only.
        /// </summary>
        /// <param name="id">The id of the clinic to deactivate.</param>
        /// <response code="204">The clinic was deactivated.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Clinic not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeactivateClinicById(int id)
        {
            if (!await _clinicService.Exists(id))
            {
                return NotFound();
            }

            await _clinicService.DeactivateClinic(id);
            return NoContent();
        }

        /// <summary>
        /// Reactivates a previously deactivated clinic. Admin only.
        /// </summary>
        /// <param name="id">The id of the clinic to reactivate.</param>
        /// <response code="204">The clinic was reactivated.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Clinic not found.</response>
        [HttpPatch("{id}/reactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReactivateClinicById(int id)
        {
            if (!await _clinicService.Exists(id))
            {
                return NotFound();
            }

            await _clinicService.ReactivateClinic(id);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing clinic. Admin only.
        /// </summary>
        /// <param name="id">The id from the route.</param>
        /// <param name="clinicData">The updated clinic data. The Id field must match the route id.</param>
        /// <response code="204">The clinic was updated.</response>
        /// <response code="400">The request body is invalid or the route id does not match the body id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Clinic not found.</response>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClinicById(int id, [FromBody] UpdateClinicRequestDto clinicData)
        {

            if (id != clinicData.Id)
            {
                return BadRequest("Id in route does not match the Id in the request body");
            }

            if (!await _clinicService.Exists(clinicData.Id))
            {
                return NotFound();
            }

            await _clinicService.UpdateClinic(clinicData);
            return NoContent();
        }

    }
}
