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
        public async Task<ActionResult<IEnumerable<ClinicResponseDto>>> GetallClinics()
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
            if(! await _clinicService.Exists(id))
            {
                return NotFound();
            }

            await _clinicService.UpdateClinic(id, clinicData);
            return NoContent();
        }

    }
}
