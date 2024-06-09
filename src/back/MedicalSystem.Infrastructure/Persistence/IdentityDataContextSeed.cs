using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace MedicalSystem.Infrastructure.Persistence
{
    public class IdentityDataContextSeed
    {
        private readonly IUserService _userService;

        public static async Task SeedAsync(IdentityDataContext dbContext, ILogger<IdentityDataContextSeed> logger, IUserService userService)
        {

            logger.LogInformation("Seed database associated with context {DbContextName}", typeof(IdentityDataContext).Name);

            await userService.CreateRolesAsync();

            if(!dbContext.Users.Any(a => a.Email == "admin@email.com"))
            {
                var user = new UserCreateRequestDto
                {
                    Email = "admin@email.com",
                    Password = "1qaz!QAZ",
                    PasswordConfirmation = "1qaz!QAZ",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Role = UserRoles.Admin

                };
                await userService.CreateAdminUserAsync(user);
            }
            
            if(!dbContext.Users.Any(a => a.Email == "patient@email.com"))
            {
                var patient = new UserCreateRequestDto
                {
                    Email = "patient@email.com",
                    Password = "1qaz!QAZ",
                    PasswordConfirmation = "1qaz!QAZ",
                    FirstName = "Patient",
                    LastName = "Patient",
                    Role = UserRoles.Patient,
                    CPF = "123.456.789-09",
                    BirthDate = new DateTime(1994, 9, 16)
                };
                await userService.CreateUserAsync(patient);
            }
            
            if(!dbContext.Users.Any(a => a.Email == "doctor@email.com"))         
            {
                var doctor = new UserCreateRequestDto
                {
                    Email = "doctor@email.com",
                    Password = "1qaz!QAZ",
                    PasswordConfirmation = "1qaz!QAZ",
                    FirstName = "Doctor",
                    LastName = "Doctor",
                    Role = UserRoles.Doctor,
                    CRM = "999999",
                    Speciality = SpecialityEnum.Pediatrician
                };
                await userService.CreateUserAsync(doctor);
            }
            
        }

    }
}
