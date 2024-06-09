using MedicalSystem.Application.Dtos.Doctor;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSystem.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(ILogger<DoctorController> logger, IDoctorService doctorService)
        {
            _logger = logger;
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<ActionResult<IList<DoctorResponceDto>>> GetAll()
        {
            try
            {
                var result = await _doctorService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetAll)} Exception during doctor get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpGet]
        [Route("getbyId/{id}")]
        public async Task<ActionResult<DoctorResponceDto>> GetById(string id)
        {
            try
            {
                var result = await _doctorService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetById)} Exception during doctor get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpGet]
        [Route("getbySpeciality/{speciality}")]
        public async Task<ActionResult<IList<DoctorResponceDto>>> GetBySpeciality(SpecialityEnum speciality)
        {
            try
            {
                var result = await _doctorService.GetBySpeciality(speciality);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetBySpeciality)} Exception during doctor get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpPut]
        public async Task<ActionResult<DoctorResponceDto>> Update([FromBody] DoctorUpdateRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _doctorService.UpdateAsync(model);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Update)} Exception during doctor update!.", ex);
                return BadRequest("Exception!");
            }
        }

    }
}