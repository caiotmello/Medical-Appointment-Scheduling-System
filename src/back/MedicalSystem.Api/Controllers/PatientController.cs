using MedicalSystem.Application.Dtos.Patient;
using MedicalSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MedicalSystem.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/patients")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(ILogger<PatientController> logger, IPatientService PatientService)
        {
            _logger = logger;
            _patientService = PatientService;
        }

        [HttpGet]
        public async Task<ActionResult<IList<PatientResponceDto>>> GetAll()
        {
            try
            {
                var result = await _patientService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetAll)} Exception during Patient get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpGet]
        [Route("getbyId/{id}")]
        public async Task<ActionResult<PatientResponceDto>> GetById(string id)
        {
            try
            {
                var result = await _patientService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetById)} Exception during Patient get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpPut]
        public async Task<ActionResult<PatientResponceDto>> Update([FromBody] PatientUpdateRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _patientService.UpdateAsync(model);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Update)} Exception during Patient update!.", ex);
                return BadRequest("Exception!");
            }
        }

    }
}
