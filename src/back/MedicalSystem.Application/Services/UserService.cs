using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using MedicalSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MedicalSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<IUserService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;

        public UserService(ILogger<IUserService> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtService jwtService, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
        }

        public async Task CreateUserAsync(UserCreateRequestDto model)
        {
            if (model == null)
                return;


            if(model.Role == UserRoles.Patient)
            {
                var patientUser = new Patient
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CPF = model.CPF,
                    BirthDate = model.BirthDate
                };

                var resultt = await _userManager.CreateAsync(patientUser, model.Password);
                if (!resultt.Succeeded)
                {
                    _logger.LogError($"{nameof(CreateUserAsync)} registration invalid!. User details: {model.Email}");
                    throw new Exception($"{nameof(CreateUserAsync)} registration invalid!. User details: {model.Email}");
                }

                return;
            }

            if (model.Role == UserRoles.Doctor)
            {
                var doctorUser = new Doctor
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CRM = model.CRM,
                    Speciality = model.Speciality
                };

                var resultt = await _userManager.CreateAsync(doctorUser, model.Password);
                if (!resultt.Succeeded)
                {
                    _logger.LogError($"{nameof(CreateUserAsync)} registration invalid!. User details: {model.Email}");
                    throw new Exception($"{nameof(CreateUserAsync)} registration invalid!. User details: {model.Email}");
                }

                return;
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogError($"{nameof(CreateUserAsync)} registration invalid!. User details: {model.Email}");
                throw new Exception($"{nameof(CreateUserAsync)} registration invalid!. User details: {model.Email}");
            }

            return;

        }

        public async Task CreateAdminUserAsync(UserCreateRequestDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null)
                throw new Exception("User already exists!");

            ApplicationUser user = new()
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogError($"{nameof(CreateAdminUserAsync)} registration invalid!. User details: {model.Email}");
                throw new Exception($"{nameof(CreateAdminUserAsync)} registration invalid!. User details: {model.Email}");
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (!await _roleManager.RoleExistsAsync(UserRoles.Doctor))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Doctor));

            if (!await _roleManager.RoleExistsAsync(UserRoles.Patient))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Patient));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _userManager.AddToRoleAsync(user, UserRoles.Doctor);

            if (await _roleManager.RoleExistsAsync(UserRoles.Patient))
                await _userManager.AddToRoleAsync(user, UserRoles.Patient);

        }
        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                _logger.LogError("user not found to be deleted");
                return;
            }

            await _userManager.DeleteAsync(user);
        }

        public List<UserResponseDto> GetUsersList()
        {
            var users = _userManager.Users.ToList();

            var userDtos = users.Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            }).ToList();

            return userDtos;

    }

        public async Task<UserTokenResponseDto> LoginAsync(UserLoginRequestDto model)
        {
            if (model is null)
                throw new ArgumentNullException();

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);


            var user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(user);

            if (!result.Succeeded)
                throw new Exception("Failed to login this user");

            var token = await _jwtService.GenerateToken(user, roles);

            return token;

        }

        public async Task<UserTokenResponseDto> RefreshTokenAsync(UserTokenRequestDto tokenModel)
        {
            if (tokenModel is null)
                throw new InvalidUserException();

            return await _jwtService.RefreshToken(tokenModel);
        }

        public async Task<bool> ResetPasswordAsync(UserResetPasswordRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                throw new Exception("not found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            return result.Succeeded;
        }

        public async Task SoftDeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is  null)
            {
                _logger.LogError("user not found to be deleted");
                return;
            }

            user.Status = UserStatusEnum.Inactive;
            await _userManager.UpdateAsync(user);
            
            _logger.LogError("user status set to inactive");
            return;

        }
    
        public async Task RevokeTokenAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
                throw new InvalidUserException();

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task RevokeAllTokenAsync()
        {
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                try
                {
                    user.RefreshToken = null;
                    await _userManager.UpdateAsync(user);
                }
                catch (Exception e)
                {
                    _logger.LogError($"error revoking credentials for {user.Email}", e);
                    throw new Exception($"error revoking credentials for {user.Email}");
                }
            }
        }

        public async Task CreateRolesAsync()
        {
            bool x = await _roleManager.RoleExistsAsync(UserRoles.Admin);
            if (!x)
            {
                // first we create Admin rool
                var role = new IdentityRole();
                role.Name = "Admin";
                await _roleManager.CreateAsync(role);
            }

            // creating Creating Manager role
            x = await _roleManager.RoleExistsAsync(UserRoles.Doctor);
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Doctor";
                await _roleManager.CreateAsync(role);
            }

            // creating Creating Employee role
            x = await _roleManager.RoleExistsAsync(UserRoles.Patient);
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Patient";
                await _roleManager.CreateAsync(role);
            }
        }

    }
}
