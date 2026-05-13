using ClinicAPI.Models.Dtos.Clinic;
using ClinicAPI.Models.Entities;
using ClinicAPI.Services.Clinics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Controllers
{
    [Route("api/v1/clinics")]
    [ApiController]
    public class ClinicController : ControllerBase
    {
        private readonly IClinicService _clinicService;

        public ClinicController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicResponseDto>>> GetAllClinics()
        {
            var clinics = await _clinicService.GetAllAsync();
            return Ok(clinics);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicResponseDto>> GetClinicById(int id)
        {
            var clinic = await _clinicService.GetByIdAsync(id);

            if(clinic == null)
            {
                return NotFound();
            }

            return Ok(clinic);
        }

        [HttpPost]
        public async Task<ActionResult<ClinicResponseDto>> CreateClinic([FromBody] CreateClinicRequestDto clinicData)
        {
            var clinic = await _clinicService.AddClinic(clinicData);
            return CreatedAtAction(nameof(GetClinicById), new {id = clinic.Id}, clinic);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteteClinicById(int id)
        {
            if(! await _clinicService.Exists(id))
            {
                return NotFound();
            }

            await _clinicService.DeleteClinic(id);
            return NoContent();
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinicById(int id, [FromBody] UpdateClinicRequestDto clinicData)
        {

            if(id != clinicData.Id)
            {
                return BadRequest("Id in route does not match the Id in the request body");
            }

            if(! await _clinicService.Exists(clinicData.Id))
            {
                return NotFound();
            }

            await _clinicService.UpdateClinic(clinicData);
            return NoContent();
        }

    }
}
