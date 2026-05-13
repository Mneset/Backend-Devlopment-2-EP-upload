using ClinicAPI.Models.Dtos.AppointmentCategory;
using ClinicAPI.Services.AppointmentCategories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/appointmentcategories")]
    [ApiController]
    public class AppointmentCategoryController : ControllerBase
    {
        private readonly IAppointmentCategoryService _appointmentCategoryService;

        public AppointmentCategoryController(IAppointmentCategoryService appointmentCategoryService)
        {
            _appointmentCategoryService = appointmentCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentCategoryResponseDto>>> GetllAppointmentCategories()
        {
            var appointmentCategories = await _appointmentCategoryService.GetAllAsync();
            return Ok(appointmentCategories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentCategoryResponseDto>> GetAppointmentCategoryById(int id)
        {
            var appointmentCategory = await _appointmentCategoryService.GetByIdAsync(id);

            if(appointmentCategory == null)
            {
                return NotFound();
            }

            return Ok(appointmentCategory);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentCategoryResponseDto>> CreateAppointmentCategory([FromBody] CreateAppointmentCategoryRequestDto appointmentCategoryData)
        {
            var appointmentCategory = await _appointmentCategoryService.AddAppointmentCategory(appointmentCategoryData);
            return CreatedAtAction(nameof(GetAppointmentCategoryById), new {id = appointmentCategory.Id}, appointmentCategory);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointMentCategoryById(int id)
        {
            if(! await _appointmentCategoryService.Exists(id))
            {
                return NotFound();
            }

            await _appointmentCategoryService.DeleteAppointmentCategory(id);
            return NoContent();
        }

        
        [HttpPut("{id}")]
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
