using MedicalSystem.Application.Configurations;
using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Application.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace MedicalSystem.Test.Application.Services
{
    public class JwtServiceTest
    {
        private readonly Mock<IOptions<JwtOptions>> _mockJwtOptions;
        private readonly Mock<ILogger<JwtService>> _mockLogger;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly JwtService _jwtService;

        public JwtServiceTest()
        {
            _mockJwtOptions = new Mock<IOptions<JwtOptions>>();
            _mockLogger = new Mock<ILogger<JwtService>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var jwtOptions = new JwtOptions
            {
                Issuer = "issuer",
                Audience = "audience",
                TokenValidityInMinutes = 60,
                RefreshTokenValidityInMinutes = 1440,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A494384E-8732-434C-AC6A-1DBE3396B9881")), SecurityAlgorithms.HmacSha256),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A494384E-8732-434C-AC6A-1DBE3396B9881"))
            };

            _mockJwtOptions.Setup(o => o.Value).Returns(jwtOptions);
            _jwtService = new JwtService(_mockJwtOptions.Object, _mockLogger.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task GenerateToken_ShouldReturnValidUserTokenResponseDto()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "test@example.com" };
            var roles = new List<string> { "Admin" };
            _mockUserManager.Setup(m => m.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles);
            _mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _jwtService.GenerateToken(user, roles);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.Expiration > DateTime.UtcNow);
        }

        [Fact]
        public void CreateToken_ShouldReturnValidJwtSecurityToken()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, "1") };

            // Act
            var token = _jwtService.CreateToken(claims);

            // Assert
            Assert.NotNull(token);
            Assert.Equal(_mockJwtOptions.Object.Value.Issuer, token.Issuer);
            Assert.True(token.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public async Task GenerateClaimsAsync_ShouldReturnValidClaims()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "test@example.com" };
            var roles = new List<string> { "Admin" };
            _mockUserManager.Setup(m => m.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles);

            // Act
            var claims = await _jwtService.GenerateClaimsAsync(user);

            // Assert
            Assert.NotNull(claims);
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidRefreshToken()
        {
            // Act
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Assert
            Assert.NotNull(refreshToken);
            Assert.Equal(88, refreshToken.Length); // Base64 string length for 64 bytes of data
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_ShouldReturnValidPrincipal()
        {

            // Arrange
            var token = _jwtService.CreateToken(new List<Claim> 
            { 
                new Claim(JwtRegisteredClaimNames.Sub, "555"),
                new Claim(JwtRegisteredClaimNames.Email, "teste@email.com"),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString())
            });
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Act
            var principal = _jwtService.GetPrincipalFromExpiredToken(tokenString);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal("555", principal.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", UserName = "testUser", RefreshToken = "validRefreshToken", RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10) };
            var token = _jwtService.CreateToken(new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.Id), new Claim(ClaimTypes.Name, user.UserName) });
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var model = new UserTokenRequestDto { AccessToken = accessToken, RefreshToken = "validRefreshToken" };

            _mockUserManager.Setup(m => m.FindByNameAsync(user.UserName)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _jwtService.RefreshToken(model);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.Expiration > DateTime.UtcNow);
        }

        [Fact]
        public async Task RefreshToken_ShouldThrowInvalidUserException_WhenPrincipalIsNull()
        {
            // Arrange
            var model = new UserTokenRequestDto { AccessToken = "invalidToken", RefreshToken = "validRefreshToken" };

            // Act & Assert
            await Assert.ThrowsAsync<SecurityTokenMalformedException>(() => _jwtService.RefreshToken(model));
        }

        [Fact]
        public async Task RefreshToken_ShouldThrowInvalidUserException_WhenUsernameIsNull()
        {
            // Arrange
            var token = _jwtService.CreateToken(new List<Claim>());
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var model = new UserTokenRequestDto { AccessToken = accessToken, RefreshToken = "validRefreshToken" };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidUserException>(() => _jwtService.RefreshToken(model));
        }

        [Fact]
        public async Task RefreshToken_ShouldThrowInvalidCredentialException_WhenUserIsNull()
        {
            // Arrange
            var token = _jwtService.CreateToken(new List<Claim> { new Claim(ClaimTypes.Name, "testUser") });
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var model = new UserTokenRequestDto { AccessToken = accessToken, RefreshToken = "validRefreshToken" };

            _mockUserManager.Setup(m => m.FindByNameAsync("testUser")).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialException>(() => _jwtService.RefreshToken(model));
        }

        [Fact]
        public async Task RefreshToken_ShouldThrowInvalidCredentialException_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", UserName = "testUser", RefreshToken = "invalidRefreshToken", RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10) };
            var token = _jwtService.CreateToken(new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.Id), new Claim(ClaimTypes.Name, user.UserName) });
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var model = new UserTokenRequestDto { AccessToken = accessToken, RefreshToken = "validRefreshToken" };

            _mockUserManager.Setup(m => m.FindByNameAsync(user.UserName)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialException>(() => _jwtService.RefreshToken(model));
        }

        [Fact]
        public async Task RefreshToken_ShouldThrowInvalidCredentialException_WhenRefreshTokenIsExpired()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", UserName = "testUser", RefreshToken = "validRefreshToken", RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-2000) };
            var token = _jwtService.CreateToken(new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.Id), new Claim(ClaimTypes.Name, user.UserName) });
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var model = new UserTokenRequestDto { AccessToken = accessToken, RefreshToken = "validRefreshToken" };

            _mockUserManager.Setup(m => m.FindByNameAsync(user.UserName)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialException>(() => _jwtService.RefreshToken(model));
        }
    }
}
