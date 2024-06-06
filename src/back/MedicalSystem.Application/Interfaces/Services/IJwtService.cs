using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Application.Interfaces.Services
{
    public interface IJwtService
    {
        /// <summary>
        /// Generates the <see cref="UserToken"/> token
        /// </summary>
        /// <param name="user">User to authenticate</param>
        /// <param name="roles">Roles of the user</param>
        /// <returns>The token</returns>
        Task<UserTokenResponseDto> GenerateToken(ApplicationUser user, IEnumerable<string> roles);

        /// <summary>
        /// Gets principal claim from the expired token
        /// </summary>
        /// <param name="token">Expired token</param>
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);

        /// <summary>
        /// Creates a new token
        /// </summary>
        /// <param name="authClaims">Claims to use on token</param>
        JwtSecurityToken CreateToken(IEnumerable<Claim> authClaims);

        /// <summary>
        /// Generates new refresh token
        /// </summary>
        string GenerateRefreshToken();

        Task<UserTokenResponseDto> RefreshToken(UserTokenRequestDto model);

        Task<IList<Claim>> GenerateClaimsAsync(ApplicationUser user);
    }
}
