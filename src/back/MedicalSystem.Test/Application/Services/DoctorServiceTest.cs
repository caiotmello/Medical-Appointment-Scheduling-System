using MedicalSystem.Application.Dtos.Doctor;
using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Application.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace MedicalSystem.Test.Application.Services
{
    public class DoctorServiceTest
    {
        private readonly Mock<ILogger<DoctorService>> _mockLogger;
        private readonly Mock<IDoctorRepository> _mockRepository;
        private readonly DoctorService _service;

        public DoctorServiceTest()
        {
            _mockLogger = new Mock<ILogger<DoctorService>>();
            _mockRepository = new Mock<IDoctorRepository>();
            _service = new DoctorService(_mockLogger.Object, _mockRepository.Object);
        }

        [Fact(DisplayName = "[UnitTest] DoctorService - Should return active doctors")]
        [Trait("Doctor", "DoctorService Validation")]
        public async Task GetAllAsync_ShouldReturnActiveDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Id = "1", FirstName = "John", LastName = "Doe", Status = UserStatusEnum.Active },
                new Doctor { Id = "2", FirstName = "Jane", LastName = "Doe", Status = UserStatusEnum.Inactive }
            };
            _mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Doctor, bool>>>()))
                           .ReturnsAsync(doctors.Where(d => d.Status != UserStatusEnum.Inactive).ToList());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result.First().FirstName);
        }

        [Fact(DisplayName = "[UnitTest] DoctorService - Should return doctor when Id matches")]
        [Trait("Doctor", "DoctorService Validation")]
        public async Task GetByIdAsync_ShouldReturnDoctor_WhenIdMatches()
        {
            // Arrange
            var doctor = new Doctor { Id = "1", FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(doctor);

            // Act
            var result = await _service.GetByIdAsync("1");

            // Assert
            Assert.Equal("John", result.FirstName);
        }

        [Fact(DisplayName = "[UnitTest] DoctorService - Should throw exception doctor Id doesn't matches")]
        [Trait("Doctor", "DoctorService Validation")]
        public async Task GetByIdAsync_ShouldThrowArgumentException_WhenDoctorNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Doctor)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync("1"));
        }

        [Fact(DisplayName = "[UnitTest] DoctorService - Should return doctor which match speciality")]
        [Trait("Doctor", "DoctorService Validation")]
        public async Task GetBySpeciality_ShouldReturnDoctors_WithMatchingSpeciality()
        {
            // Arrange
            var doctors = new List<Doctor>
        {
            new Doctor { Id = "1", FirstName = "John", LastName = "Doe", Speciality = SpecialityEnum.Cardiologist },
            new Doctor { Id = "2", FirstName = "Jane", LastName = "Doe", Speciality = SpecialityEnum.Pediatrician }
        };
            _mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Doctor, bool>>>()))
                           .ReturnsAsync(doctors.Where(d => d.Speciality == SpecialityEnum.Cardiologist).ToList());

            // Act
            var result = await _service.GetBySpeciality(SpecialityEnum.Cardiologist);

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result.First().FirstName);
        }

        [Fact(DisplayName = "[UnitTest] DoctorService - Should update doctor with success")]
        [Trait("Doctor", "DoctorService Validation")]
        public async Task UpdateAsync_ShouldUpdateDoctor()
        {
            // Arrange
            var doctor = new Doctor { Id = "1", FirstName = "John", LastName = "Doe", CRM = "123" };
            var updateModel = new DoctorUpdateRequestDto
            {
                Id = "1",
                FirstName = "John Updated",
                LastName = "Doe Updated",
                CRM = "456",
                PhoneNumber = "1234567890"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(doctor);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(updateModel);

            // Assert
            Assert.Equal("John Updated", result.FirstName);
            Assert.Equal("Doe Updated", result.LastName);
            Assert.Equal("456", result.CRM);
            Assert.Equal("1234567890", result.PhoneNumber);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Doctor>()), Times.Once);
        }

        [Fact(DisplayName = "[UnitTest] DoctorService - Should throw exception when doctor Id doesn't during update")]
        [Trait("Doctor", "DoctorService Validation")]
        public async Task UpdateAsync_ShouldThrowException_WhenDoctorNotFound()
        {
            // Arrange
            var updateModel = new DoctorUpdateRequestDto { Id = "1" };
            _mockRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Doctor)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(updateModel));
        }

        [Fact(DisplayName = "[UnitTest] DoctorService - Should throw exception when model is null")]
        [Trait("Doctor", "DoctorService Validation")]
        public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null));
        }
    }
}
