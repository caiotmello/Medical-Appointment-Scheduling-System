using MedicalSystem.Application.Configurations;
using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MedicalSystem.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<JwtService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtService(IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger, UserManager<ApplicationUser> userManager)
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<UserTokenResponseDto> GenerateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var claims = await this.GenerateClaimsAsync(user);
            var token = CreateToken(claims);
            var refreshToken = GenerateRefreshToken();


            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(_jwtOptions.RefreshTokenValidityInMinutes);
            await _userManager.UpdateAsync(user);

            return new UserTokenResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };
        }

        public JwtSecurityToken CreateToken(IEnumerable<Claim> authClaims)
        {
            var token = new JwtSecurityToken
            (
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: authClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_jwtOptions.TokenValidityInMinutes),
                signingCredentials: _jwtOptions.SigningCredentials
                );

            return token;
        }

        public async Task<IList<Claim>> GenerateClaimsAsync(ApplicationUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
            long iat = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, iat.ToString()));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            return claims;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtOptions.IssuerSigningKey,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();


            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<UserTokenResponseDto> RefreshToken(UserTokenRequestDto model)
        {
            var accessToken = model.AccessToken;
            var refreshToken = model.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal is null)
                throw new InvalidUserException();

            var username = principal.Identity?.Name;
            if (username is null)
                throw new InvalidUserException();

            var user = await _userManager.FindByNameAsync(username);

            if (user is null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new InvalidCredentialException();
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new UserTokenResponseDto
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(newAccessToken),
                RefreshToken = newRefreshToken,
                Expiration = newAccessToken.ValidTo
            }; 
        }
    }
}
