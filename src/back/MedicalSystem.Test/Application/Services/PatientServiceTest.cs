using MedicalSystem.Application.Dtos.Patient;
using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Application.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace MedicalSystem.Test.Application.Services
{
    public class PatientServiceTest
    {
        private readonly Mock<ILogger<PatientService>> _mockLogger;
        private readonly Mock<IPatientRepository> _mockRepository;
        private readonly PatientService _service;

        public PatientServiceTest()
        {
            _mockLogger = new Mock<ILogger<PatientService>>();
            _mockRepository = new Mock<IPatientRepository>();
            _service = new PatientService(_mockLogger.Object, _mockRepository.Object);
        }

        [Fact(DisplayName = "[UnitTest] GetAllAsync - Should return active patients")]
        [Trait("Patient", "PatientService Validation")]
        public async Task GetAllAsync_ShouldReturnActivePatients()
        {
            // Arrange
            var patients = new List<Patient>
        {
            new Patient { Id = "1", FirstName = "John", LastName = "Doe", Status = UserStatusEnum.Active },
            new Patient { Id = "2", FirstName = "Jane", LastName = "Doe", Status = UserStatusEnum.Inactive }
        };
            _mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Patient, bool>>>()))
                           .ReturnsAsync(patients.Where(p => p.Status != UserStatusEnum.Inactive).ToList());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result.First().FirstName);
        }

        [Fact(DisplayName = "[UnitTest] GetByIdAsync - Should return patient when Id matches")]
        [Trait("Patient", "PatientService Validation")]
        public async Task GetByIdAsync_ShouldReturnPatient_WhenIdMatches()
        {
            // Arrange
            var patient = new Patient { Id = "1", FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(patient);

            // Act
            var result = await _service.GetByIdAsync("1");

            // Assert
            Assert.Equal("John", result.FirstName);
        }

        [Fact(DisplayName = "[UnitTest] GetByIdAsync - Should return exception when patient not found")]
        [Trait("Patient", "PatientService Validation")]
        public async Task GetByIdAsync_ShouldThrowArgumentException_WhenPatientNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Patient)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync("1"));
        }

        [Fact(DisplayName = "[UnitTest] UpdateAsync - Should update patient with success")]
        [Trait("Patient", "PatientService Validation")]
        public async Task UpdateAsync_ShouldUpdatePatient()
        {
            // Arrange
            var patient = new Patient { Id = "1", FirstName = "John", LastName = "Doe", CPF = "123" };
            var updateModel = new PatientUpdateRequestDto
            {
                Id = "1",
                FirstName = "John Updated",
                LastName = "Doe Updated",
                CPF = "456",
                PhoneNumber = "1234567890"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(patient);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(updateModel);

            // Assert
            Assert.Equal("John Updated", result.FirstName);
            Assert.Equal("Doe Updated", result.LastName);
            Assert.Equal("456", result.CPF);
            Assert.Equal("1234567890", result.PhoneNumber);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Patient>()), Times.Once);
        }

        [Fact(DisplayName = "[UnitTest] UpdateAsync - Should throw exception when patient not found")]
        [Trait("Patient", "PatientService Validation")]
        public async Task UpdateAsync_ShouldThrowException_WhenPatientNotFound()
        {
            // Arrange
            var updateModel = new PatientUpdateRequestDto { Id = "1" };
            _mockRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Patient)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(updateModel));
        }

        [Fact(DisplayName = "[UnitTest] UpdateAsync - Should throw exception when model is null")]
        [Trait("Patient", "PatientService Validation")]
        public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null));
        }

    }
}
