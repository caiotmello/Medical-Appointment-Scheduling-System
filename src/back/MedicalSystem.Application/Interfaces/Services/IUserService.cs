using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserTokenResponseDto> LoginAsync(UserLoginRequestDto model);

        Task<bool> ResetPasswordAsync(UserResetPasswordRequestDto  model);

        Task CreateUserAsync(UserCreateRequestDto model);

        Task CreateAdminUserAsync(UserCreateRequestDto model);

        Task DeleteUserAsync(string id);

        Task SoftDeleteUserAsync(string id);

        Task<UserTokenResponseDto> RefreshTokenAsync(UserTokenRequestDto tokenModel);

        Task RevokeTokenAsync(string userEmail);

        Task RevokeAllTokenAsync();
        List<UserResponseDto> GetUsersList();

        Task CreateRolesAsync();

    }
}
