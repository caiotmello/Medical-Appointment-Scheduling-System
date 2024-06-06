using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Exceptions;
using MedicalSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace MedicalSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController: ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UserController(ILogger<UserController> logger, IUserService userService, IEmailService emailService)
        {
            _logger = logger;
            _userService = userService;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserTokenResponseDto>> Login([FromBody] UserLoginRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.LoginAsync(model);
                return result;
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Invalid login!");
                _logger.LogWarning($"{nameof(User)} login invalid!. User details: {model.Email}", e);
                return BadRequest(ModelState);
            }
        }


        [HttpPost("reset")]
        [Authorize]
        public async Task<ActionResult<UserTokenResponseDto>> ResetPassword([FromBody] UserResetPasswordRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var loggedUser = GetCurrentUser();
                if (loggedUser != model.Email)
                    return StatusCode(500);

                var result = await _userService.ResetPasswordAsync(model);
                return result
                    ? Ok()
                    : BadRequest();
            }
            catch(Exception e)
            {
                _logger.LogWarning($"{nameof(User)} password reset error!. User details: {model.Email}", e);
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<ActionResult<UserTokenResponseDto>> RefreshToken(UserTokenRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return await _userService.RefreshTokenAsync(model);
            }
            catch (InvalidUserException e)
            {
                _logger.LogError($"Refreshing token", e);
                return Forbid("Invalid user");
            }
            catch (InvalidCredentialException e)
            {
                _logger.LogError($"Refreshing token", e);
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPost]
        [Route("revoke/{useremail}")]
        public async Task<IActionResult> Revoke(string useremail)
        {
            var currentUserEmail = GetCurrentUserEmail();
            if (useremail != currentUserEmail)
                return Unauthorized();

            try
            {
                await _userService.RevokeTokenAsync(useremail);
            }
            catch (Exception e)
            {
                _logger.LogError($"error revoking credentials for {useremail}", e);
            }

            return NoContent();

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> Revoke()
        {
            try
            {
                await _userService.RevokeAllTokenAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"error revoking all credentials");
            }

            return NoContent();

        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> CreateUser([FromBody] UserCreateRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _userService.CreateUserAsync(model);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(CreateUser)} Exception during registration!.", e);
                return BadRequest("Exception!");
            }

        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<ActionResult> CreateAdminUser([FromBody] UserCreateRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(model.Role != "Admin")
                return BadRequest(ModelState);
            
            try
            {
                await _userService.CreateAdminUserAsync(model);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(CreateUser)} Exception during admin registration!.", e);
                return BadRequest("Exception!");
            }

        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _userService.SoftDeleteUserAsync(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("[Delete User] - Exception trying to delete user", e);
                return StatusCode(500);
            }
        }

        private string? GetCurrentUser() =>
            User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        private string GetCurrentUserEmail() => 
            User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    }
}
