using ClinicAPI.Models.Dtos.Speciality;
using ClinicAPI.Services.Specialtities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/specialities")]
    [ApiController]
    public class SpecialityController : ControllerBase
    {
        private readonly ISpecialityService _specialityService;

        public SpecialityController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialityResponseDto>>> GetAllSpecialities()
        {
            var specialities = await _specialityService.GetAllAsync();
            return Ok(specialities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialityResponseDto>> GetSpecialityById(int id)
        {
            var speciality = await _specialityService.GetByIdAsync(id);

            if(speciality == null)
            {
                return NotFound();
            }

            return Ok(speciality);
        }

        [HttpPost]
        public async Task<ActionResult<SpecialityResponseDto>> CreateSpeciality([FromBody] CreateSpecialityRequestDto specialityData)
        {
            var speciality = await _specialityService.AddSpeciality(specialityData);
            return CreatedAtAction(nameof(GetSpecialityById), new {id = speciality.Id}, speciality);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteteSpecialityById(int id)
        {
            if(! await _specialityService.Exists(id))
            {
                return NotFound();
            }

            await _specialityService.DeleteSpeciality(id);
            return NoContent();
        }



        [HttpPut("{id}")]
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
