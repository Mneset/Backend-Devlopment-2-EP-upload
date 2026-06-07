using ClinicAPI.Models.Dtos.Speciality;
using ClinicAPI.Services.Specialities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/specialities")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SpecialityController : ControllerBase
    {
        private readonly ISpecialityService _specialityService;

        public SpecialityController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }

        /// <summary>
        /// Gets all specialities. Public.
        /// </summary>
        /// <response code="200">Returns the list of specialities.</response>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<SpecialityResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SpecialityResponseDto>>> GetAllSpecialities()
        {
            var specialities = await _specialityService.GetAllAsync();
            return Ok(specialities);
        }

        /// <summary>
        /// Gets a speciality by its id. Public.
        /// </summary>
        /// <param name="id">The id of the speciality to retrieve.</param>
        /// <response code="200">Returns the speciality with the specified id.</response>
        /// <response code="404">Speciality not found.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SpecialityResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SpecialityResponseDto>> GetSpecialityById(int id)
        {
            var speciality = await _specialityService.GetByIdAsync(id);

            if(speciality == null)
            {
                return NotFound();
            }

            return Ok(speciality);
        }

        /// <summary>
        /// Creates a new speciality. Admin only.
        /// </summary>
        /// <param name="specialityData">The speciality data to create.</param>
        /// <response code="201">The speciality was created.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SpecialityResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SpecialityResponseDto>> CreateSpeciality([FromBody] CreateSpecialityRequestDto specialityData)
        {
            var speciality = await _specialityService.AddSpeciality(specialityData);
            return CreatedAtAction(nameof(GetSpecialityById), new {id = speciality.Id}, speciality);
        }

        /// <summary>
        /// Deletes a speciality by its id. Admin only. Fails if any doctors are assigned to it.
        /// </summary>
        /// <param name="id">The id of the speciality to delete.</param>
        /// <response code="204">The speciality was deleted.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Speciality not found.</response>
        /// <response code="409">The speciality has doctors assigned to it and cannot be deleted.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteSpecialityById(int id)
        {
            if(! await _specialityService.Exists(id))
            {
                return NotFound();
            }

            if(await _specialityService.HasDoctors(id))
            {
                return Conflict("Cannot delete speciality because doctors are assigned to it.");
            }

            await _specialityService.DeleteSpeciality(id);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing speciality. Admin only.
        /// </summary>
        /// <param name="id">The id from the route.</param>
        /// <param name="specialityData">The updated speciality data. The Id field must match the route id.</param>
        /// <response code="204">The speciality was updated.</response>
        /// <response code="400">The request body is invalid or the route id does not match the body id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Speciality not found.</response>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSpecialityById(int id, [FromBody] UpdateSpecialityRequestDto specialityData)
        {

            if(id != specialityData.Id)
            {
                return BadRequest("Id in route does not match the Id in the request body");
            }

            if(! await _specialityService.Exists(specialityData.Id))
            {
                return NotFound();
            }

            await _specialityService.UpdateSpeciality(specialityData);
            return NoContent();
        }
    }
}
