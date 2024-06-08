using MedicalSystem.Application.Dtos.Appointment;
using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Application.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedicalSystem.Test.Application.Services
{
    public class AppointmentServiceTest
    {
        private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
        private readonly Mock<ILogger<IAppointmentService>> _mockLogger;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTest()
        {
            _mockAppointmentRepository = new Mock<IAppointmentRepository>();
            _mockLogger = new Mock<ILogger<IAppointmentService>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _appointmentService = new AppointmentService(_mockAppointmentRepository.Object, _mockLogger.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateAppointment_WhenModelIsValid()
        {
            // Arrange
            var model = new AppointmentCreateRequestDto
            {
                PatientId = "patientId",
                DoctorId = "doctorId",
                Date = DateTime.Today,
                Time = TimeSpan.FromHours(9)
            };

            var appointmentId = Guid.NewGuid();
            var createdAppointment = new Appointment
            {
                Id = appointmentId,
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                Date = model.Date,
                Time = model.Time,
                Status = AppoitmentStatusEnum.Scheduled
            };

            _mockAppointmentRepository.Setup(repo => repo.AddAsync(It.IsAny<Appointment>()))
                                      .ReturnsAsync(createdAppointment);

            // Act
            var result = await _appointmentService.CreateAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointmentId, result.Id);
            Assert.Equal(model.PatientId, result.PatientId);
            Assert.Equal(model.DoctorId, result.DoctorId);
            Assert.Equal(model.Date, result.Date);
            Assert.Equal(model.Time, result.Time);
            Assert.Equal(AppoitmentStatusEnum.Scheduled, result.Status);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAppointment_WhenIdExists()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { Id = appointmentId };
            _mockAppointmentRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                                      .ReturnsAsync(appointment);

            // Act
            var result = await _appointmentService.DeleteAsync(appointmentId);

            // Assert
            Assert.True(result);
            Assert.Equal(AppoitmentStatusEnum.Cancelled, appointment.Status);
            _mockAppointmentRepository.Verify(repo => repo.UpdateAsync(appointment), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
        {
            new Appointment { Id = Guid.NewGuid(), Status = AppoitmentStatusEnum.Scheduled },
            new Appointment { Id = Guid.NewGuid(), Status = AppoitmentStatusEnum.Realized }
        };
            _mockAppointmentRepository.Setup(repo => repo.GetAllAsync())
                                      .ReturnsAsync(appointments);

            // Act
            var result = await _appointmentService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, a => a.Status == AppoitmentStatusEnum.Scheduled);
            Assert.Contains(result, a => a.Status == AppoitmentStatusEnum.Realized);
        }

        [Fact]
        public async Task GeByDateAsync_ShouldReturnAppointmentsByDate()
        {
            // Arrange
            var date = DateTime.Today;
            var appointments = new List<Appointment>
            {
                new Appointment { Id = Guid.NewGuid(), Date = date, Status = AppoitmentStatusEnum.Scheduled,PatientId = "patientId", DoctorId = "doctorId" },
                new Appointment { Id = Guid.NewGuid(), Date = date.AddDays(1), Status = AppoitmentStatusEnum.Scheduled,PatientId = "patientId", DoctorId = "doctorId" }
            };

            var startDate = new DateTime(2024, 6, 1);
            var endDate = new DateTime(2024, 6, 30);

            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByDateAsync(startDate, endDate)).ReturnsAsync(appointments);

            // Act
            var result = await _appointmentService.GetByDate(startDate, endDate);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.InRange(a.Date, startDate, endDate));
        }

        [Fact]
        public async Task GetByDoctorAsync_ShouldReturnAppointmentsByDoctorId()
        {
            // Arrange
            var doctorId = "doctorId";
            var doctor = new Doctor { Id = doctorId };
            var appointments = new List<Appointment>
            {
                new Appointment { Id = Guid.NewGuid(), DoctorId = doctorId },
                new Appointment { Id = Guid.NewGuid(), DoctorId = doctorId }
            };
            _mockUserManager.Setup(manager => manager.FindByIdAsync(doctorId))
                            .ReturnsAsync(doctor);
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByDoctor(doctor))
                                      .ReturnsAsync(appointments);

            // Act
            var result = await _appointmentService.GetByDoctorAsync(doctorId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.Equal(doctorId, a.DoctorId));
        }

        [Fact]
        public async Task GetByPatientAsync_ShouldReturnAppointmentsByPatientId()
        {
            // Arrange
            var patientId = "patientId";
            var patient = new Patient { Id = patientId };
            var appointments = new List<Appointment>
            {
                new Appointment { Id = Guid.NewGuid(), PatientId = patientId },
                new Appointment { Id = Guid.NewGuid(), PatientId = patientId }
            };
            _mockUserManager.Setup(manager => manager.FindByIdAsync(patientId))
                            .ReturnsAsync(patient);
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByPatient(patient))
                .ReturnsAsync(appointments);

            // Act
            var result = await _appointmentService.GetByPatientAsync(patientId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.Equal(patientId, a.PatientId));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAppointmentById()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { Id = appointmentId };
            _mockAppointmentRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                                      .ReturnsAsync(appointment);

            // Act
            var result = await _appointmentService.GetByIdAsync(appointmentId);

            // Assert
            Assert.Equal(appointmentId, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAppointment_WhenModelIsValid()
        {
            // Arrange
            var model = new AppointmentUpdateRequestDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.AddDays(1),
                Time = TimeSpan.FromHours(10),
                Status = AppoitmentStatusEnum.Realized
            };

            var appointment = new Appointment { Id = model.Id };
            _mockAppointmentRepository.Setup(repo => repo.GetByIdAsync(model.Id))
                                      .ReturnsAsync(appointment);

            // Act
            var result = await _appointmentService.UpdateAsync(model);

            // Assert
            Assert.Equal(model.Date, result.Date);
            Assert.Equal(model.Time, result.Time);
            Assert.Equal(model.Status, result.Status);
            _mockAppointmentRepository.Verify(repo => repo.UpdateAsync(appointment), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _appointmentService.CreateAsync(null));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenAppointmentNotFound()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _mockAppointmentRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                                      .ReturnsAsync((Appointment)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _appointmentService.DeleteAsync(appointmentId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _appointmentService.UpdateAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenAppointmentNotFound()
        {
            // Arrange
            var model = new AppointmentUpdateRequestDto { Id = Guid.NewGuid() };
            _mockAppointmentRepository.Setup(repo => repo.GetByIdAsync(model.Id))
                                      .ReturnsAsync((Appointment)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _appointmentService.UpdateAsync(model));
        }

        [Fact]
        public async Task GetByDoctorAsync_ShouldThrowException_WhenDoctorNotFound()
        {
            // Arrange
            var doctorId = "doctorId";
            _mockUserManager.Setup(manager => manager.FindByIdAsync(doctorId))
                            .ReturnsAsync((Doctor)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _appointmentService.GetByDoctorAsync(doctorId));
        }

        [Fact]
        public async Task GetByPatientAsync_ShouldThrowException_WhenPatientNotFound()
        {
            // Arrange
            var patientId = "patientId";
            _mockUserManager.Setup(manager => manager.FindByIdAsync(patientId))
                            .ReturnsAsync((Patient)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _appointmentService.GetByPatientAsync(patientId));
        }

        [Fact]
        public async Task GetAppointmentByDateAsync_ShouldReturnAppointmentsWithEmail()
        {
            // Arrange
            var date = DateTime.Today;
            var appointments = new List<Appointment>
            {
                new Appointment { Id = Guid.NewGuid(), Date = date, Status = AppoitmentStatusEnum.Scheduled, PatientId = "patientId", DoctorId = "doctorId" }
            };
            var patient = new ApplicationUser { FirstName = "Patient", LastName = "One", Email = "patient@example.com" };
            var doctor = new ApplicationUser { FirstName = "Doctor", LastName = "One", Email = "doctor@example.com" };
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByDateAsync(date))
                                      .ReturnsAsync(appointments);
            _mockUserManager.Setup(manager => manager.FindByIdAsync("patientId"))
                            .ReturnsAsync(patient);
            _mockUserManager.Setup(manager => manager.FindByIdAsync("doctorId"))
                            .ReturnsAsync(doctor);

            // Act
            var result = await _appointmentService.GetAppointmentByDateAsync(date);

            // Assert
            Assert.Single(result);
            Assert.Equal(patient.Email, result[0].Patient.Email);
            Assert.Equal(doctor.Email, result[0].Doctor.Email);
        }

        [Fact]
        public async Task GetAppointmentByDateAsync_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var date = DateTime.Today;
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByDateAsync(date))
                                      .ThrowsAsync(new Exception("Repository exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _appointmentService.GetAppointmentByDateAsync(date));
            Assert.Equal("Failed to retrieve appoitments!!", exception.Message);
        }

        [Fact]
        public async Task GetAppointmentByDateAsync_ShouldThrowException_WhenUserManagerThrowsException()
        {
            // Arrange
            var date = DateTime.Today;
            var appointments = new List<Appointment>
            {
                new Appointment { Id = Guid.NewGuid(), Date = date, Status = AppoitmentStatusEnum.Scheduled, PatientId = "patientId", DoctorId = "doctorId" }
            };
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByDateAsync(date))
                                      .ReturnsAsync(appointments);
            _mockUserManager.Setup(manager => manager.FindByIdAsync(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("UserManager exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _appointmentService.GetAppointmentByDateAsync(date));
            Assert.Equal("Failed to retrieve appoitments!!", exception.Message);
        }
    }
}
