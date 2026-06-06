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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetAllDoctorsIncludingInactive()
        {
            var doctors = await _doctorService.GetAllIncludingInactiveAsync();
            return Ok(doctors);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> SearchDoctors([FromQuery] string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var doctors = await _doctorService.SearchByNameAsync(name);
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DoctorResponseDto>> GetDoctorById(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }

        [HttpGet("all/{id}")]
        public async Task<ActionResult<DoctorResponseDto>> GetDoctorByIdIncludingInactive(int id)
        {
            var doctor = await _doctorService.GetByIdIncludingInactiveAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeactivateDoctorById(int id)
        {
            if (!await _doctorService.Exists(id))
            {
                return NotFound();
            }

            await _doctorService.DeactivateDoctor(id);
            return NoContent();
        }

        [HttpPatch("{id}/reactivate")]
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
