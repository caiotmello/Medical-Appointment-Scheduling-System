using MedicalSystem.Application.Dtos.User;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Application.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using MedicalSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedicalSystem.Test.Application.Services
{
    public class UserServiceTest
    {
        private readonly Mock<ILogger<IUserService>> _loggerMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _loggerMock = new Mock<ILogger<IUserService>>();
            _userManagerMock = MockUserManager<ApplicationUser>();
            _signInManagerMock = MockSignInManager<ApplicationUser>();
            _roleManagerMock = MockRoleManager<IdentityRole>();
            _jwtServiceMock = new Mock<IJwtService>();

            _userService = new UserService(
                _loggerMock.Object,
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _jwtServiceMock.Object,
                _roleManagerMock.Object);
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            return new Mock<UserManager<TUser>>(
                new Mock<IUserStore<TUser>>().Object,
                null, null, null, null, null, null, null, null);
        }

        private static Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
        {
            return new Mock<SignInManager<TUser>>(
                MockUserManager<TUser>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                null, null, null, null);
        }

        private static Mock<RoleManager<TRole>> MockRoleManager<TRole>() where TRole : class
        {
            return new Mock<RoleManager<TRole>>(
                new Mock<IRoleStore<TRole>>().Object,
                null, null, null, null);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser_WhenModelIsValid()
        {
            // Arrange
            var model = new UserCreateRequestDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                Role = UserRoles.Patient,
                FirstName = "Test",
                LastName = "User",
                CPF = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(-20)
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Patient>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.CreateUserAsync(model);

            // Assert
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<Patient>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrowException_WhenUserManagerFails()
        {
            // Arrange
            var model = new UserCreateRequestDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                Role = UserRoles.Patient,
                FirstName = "Test",
                LastName = "User",
                CPF = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(-20)
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Patient>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(model));
        }

        [Fact]
        public async Task CreateAdminUserAsync_ShouldCreateAdminUser_WhenModelIsValid()
        {
            // Arrange
            var model = new UserCreateRequestDto
            {
                Email = "admin@example.com",
                Password = "Password123!",
                FirstName = "Admin",
                LastName = "User"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.CreateAdminUserAsync(model);

            // Assert
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Exactly(3));
        }

        [Fact]
        public async Task CreateAdminUserAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            var model = new UserCreateRequestDto
            {
                Email = "admin@example.com",
                Password = "Password123!",
                FirstName = "Admin",
                LastName = "User"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.CreateAdminUserAsync(model));
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var user = new ApplicationUser { Id = userId };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.DeleteUserAsync(userId);

            // Assert
            _userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldLogError_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            // Act
            await _userService.DeleteUserAsync(userId);

            // Assert
            _loggerMock.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("user not found to be deleted")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var model = new UserLoginRequestDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new ApplicationUser { Email = model.Email };
            var roles = new List<string> { "User" };
            var token = new UserTokenResponseDto { Token = "token" };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(
                model.Email, model.Password, false, false)).ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _jwtServiceMock.Setup(x => x.GenerateToken(user, roles)).ReturnsAsync(token);

            // Act
            var result = await _userService.LoginAsync(model);

            // Assert
            Assert.Equal(token, result);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenCredentialsAreInvalid()
        {
            // Arrange
            var model = new UserLoginRequestDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(
                model.Email, model.Password, false, false)).ReturnsAsync(SignInResult.Failed);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.LoginAsync(model));
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldResetPassword_WhenUserExists()
        {
            // Arrange
            var model = new UserResetPasswordRequestDto
            {
                Email = "test@example.com",
                NewPassword = "NewPassword123!"
            };

            var user = new ApplicationUser { Email = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("resetToken");
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, "resetToken", model.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.ResetPasswordAsync(model);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var model = new UserResetPasswordRequestDto
            {
                Email = "test@example.com",
                NewPassword = "NewPassword123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.ResetPasswordAsync(model));
        }

        [Fact]
        public async Task SoftDeleteUserAsync_ShouldSetUserStatusToInactive_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var user = new ApplicationUser { Id = userId };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.SoftDeleteUserAsync(userId);

            // Assert
            Assert.Equal(UserStatusEnum.Inactive, user.Status);
            _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteUserAsync_ShouldLogError_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            // Act
            await _userService.SoftDeleteUserAsync(userId);

            // Assert
            _loggerMock.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("user not found to be deleted")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task RevokeTokenAsync_ShouldRevokeUserToken_WhenUserExists()
        {
            // Arrange
            var userEmail = "test@example.com";
            var user = new ApplicationUser { Email = userEmail };

            _userManagerMock.Setup(x => x.FindByEmailAsync(userEmail)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.RevokeTokenAsync(userEmail);

            // Assert
            Assert.Null(user.RefreshToken);
            _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task RevokeTokenAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var userEmail = "test@example.com";

            _userManagerMock.Setup(x => x.FindByEmailAsync(userEmail)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidUserException>(() => _userService.RevokeTokenAsync(userEmail));
        }

        [Fact]
        public async Task RevokeAllTokenAsync_ShouldRevokeAllUserTokens()
        {
            // Arrange
            var users = new List<ApplicationUser>
        {
            new ApplicationUser { Email = "test1@example.com" },
            new ApplicationUser { Email = "test2@example.com" }
        };

            _userManagerMock.Setup(x => x.Users).Returns(users.AsQueryable());
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.RevokeAllTokenAsync();

            // Assert
            foreach (var user in users)
            {
                Assert.Null(user.RefreshToken);
            }
            _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Exactly(users.Count));
        }

        [Fact]
        public async Task CreateRolesAsync_ShouldCreateRoles_WhenRolesDoNotExist()
        {
            // Arrange
            _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.CreateRolesAsync();

            // Assert
            _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Exactly(3));
        }

        [Fact]
        public void GetUsersList_ShouldReturnUserResponseDtos()
        {
            // Arrange
            var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "1",
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "1234567890",
                Email = "test@example.com",
                RefreshToken = "refreshToken",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            }
        };

            _userManagerMock.Setup(x => x.Users).Returns(users.AsQueryable());

            // Act
            var result = _userService.GetUsersList();

            // Assert
            Assert.Single(result);
            var userDto = result.First();
            Assert.Equal(users.First().Id, userDto.Id);
            Assert.Equal(users.First().FirstName, userDto.FirstName);
            Assert.Equal(users.First().LastName, userDto.LastName);
            Assert.Equal(users.First().PhoneNumber, userDto.PhoneNumber);
            Assert.Equal(users.First().Email, userDto.Email);
            Assert.Equal(users.First().RefreshToken, userDto.RefreshToken);
            Assert.Equal(users.First().RefreshTokenExpiryTime, userDto.RefreshTokenExpiryTime);
        }
    }
}
