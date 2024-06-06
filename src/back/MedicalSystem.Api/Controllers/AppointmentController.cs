using MedicalSystem.Application.Dtos.Appointment;
using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSystem.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/appointments")]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentService appointmentService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<ActionResult<AppointmentResponceDto>> Create([FromBody] AppointmentCreateRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _appointmentService.CreateAsync(model);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Create)} Exception during appointment creation!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpPut]
        public async Task<ActionResult<AppointmentResponceDto>> Update([FromBody] AppointmentUpdateRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _appointmentService.UpdateAsync(model);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Create)} Exception during appointment update!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _appointmentService.DeleteAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Create)} Exception during appointment delete!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpGet]
        [Route("getbyId/{id}")]
        public async Task<ActionResult<AppointmentResponceDto>> GetById(Guid id)
        {
            try
            {
                var result = await _appointmentService.GetByIdAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetById)} Exception during appointment get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IList<AppointmentResponceDto>>> GetAll()
        {
            try
            {
                var result = await _appointmentService.GetAllAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Create)} Exception during appointment get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpGet]
        [Route("getbydoctorId/{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<IList<AppointmentResponceDto>>> GetByDoctorAsync(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _appointmentService.GetByDoctorAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Create)} Exception during appointment get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpGet]
        [Route("getbypatientId/{id}")]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<ActionResult<IList<AppointmentResponceDto>>> GetByPatientAsync(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _appointmentService.GetByPatientAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Create)} Exception during appointment get!.", ex);
                return BadRequest("Exception!");
            }
        }

        [HttpGet]
        [Route("getbydate/{startdate:datetime}/{enddate:datetime}")]
        public async Task<ActionResult<IList<AppointmentResponceDto>>> GetByPatientAsync(DateTime startdate, DateTime enddate)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _appointmentService.GetByDate(startdate, enddate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Create)} Exception during appointment get!.", ex);
                return BadRequest("Exception!");
            }
        }
        
        //GetAvailableAppointments (docID, Date) -> ver se vai ser facil de implemntar
    }
}
