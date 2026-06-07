using ClinicAPI.Models.Dtos.AppointmentCategory;
using ClinicAPI.Services.AppointmentCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/appointmentcategories")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AppointmentCategoryController : ControllerBase
    {
        private readonly IAppointmentCategoryService _appointmentCategoryService;

        public AppointmentCategoryController(IAppointmentCategoryService appointmentCategoryService)
        {
            _appointmentCategoryService = appointmentCategoryService;
        }

        /// <summary>
        /// Gets all appointment categories. Public.
        /// </summary>
        /// <response code="200">Returns the list of appointment categories.</response>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentCategoryResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppointmentCategoryResponseDto>>> GetAllAppointmentCategories()
        {
            var appointmentCategories = await _appointmentCategoryService.GetAllAsync();
            return Ok(appointmentCategories);
        }

        /// <summary>
        /// Gets an appointment category by its id. Public.
        /// </summary>
        /// <param name="id">The id of the appointment category to retrieve.</param>
        /// <response code="200">Returns the appointment category with the specified id.</response>
        /// <response code="404">Appointment category not found.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AppointmentCategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentCategoryResponseDto>> GetAppointmentCategoryById(int id)
        {
            var appointmentCategory = await _appointmentCategoryService.GetByIdAsync(id);

            if(appointmentCategory == null)
            {
                return NotFound();
            }

            return Ok(appointmentCategory);
        }

        /// <summary>
        /// Creates a new appointment category. Admin only.
        /// </summary>
        /// <param name="appointmentCategoryData">The appointment category data to create.</param>
        /// <response code="201">The appointment category was created.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AppointmentCategoryResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AppointmentCategoryResponseDto>> CreateAppointmentCategory([FromBody] CreateAppointmentCategoryRequestDto appointmentCategoryData)
        {
            var appointmentCategory = await _appointmentCategoryService.AddAppointmentCategory(appointmentCategoryData);
            return CreatedAtAction(nameof(GetAppointmentCategoryById), new {id = appointmentCategory.Id}, appointmentCategory);
        }

        /// <summary>
        /// Deletes an appointment category by its id. Admin only. Fails if any appointments are assigned to it.
        /// </summary>
        /// <param name="id">The id of the appointment category to delete.</param>
        /// <response code="204">The appointment category was deleted.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Appointment category not found.</response>
        /// <response code="409">The category has appointments assigned to it and cannot be deleted.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteAppointmentCategoryById(int id)
        {
            if(! await _appointmentCategoryService.Exists(id))
            {
                return NotFound();
            }

            if(await _appointmentCategoryService.HasAppointments(id))
            {
                return Conflict("Cannot delete appointment category because appointments are assigned to it.");
            }

            await _appointmentCategoryService.DeleteAppointmentCategory(id);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing appointment category. Admin only.
        /// </summary>
        /// <param name="id">The id from the route.</param>
        /// <param name="appointmentCategoryData">The updated appointment category data. The Id field must match the route id.</param>
        /// <response code="204">The appointment category was updated.</response>
        /// <response code="400">The request body is invalid or the route id does not match the body id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Caller is not an admin.</response>
        /// <response code="404">Appointment category not found.</response>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAppointmentCategoryById(int id, [FromBody] UpdateAppointmentCategoryRequestDto appointmentCategoryData)
        {

            if(id != appointmentCategoryData.Id)
            {
                return BadRequest("Id in route does not match the Id in the request body");
            }

            if(! await _appointmentCategoryService.Exists(appointmentCategoryData.Id))
            {
                return NotFound();
            }

            await _appointmentCategoryService.UpdateAppointmentCategory(appointmentCategoryData);
            return NoContent();
        }

    }
}
