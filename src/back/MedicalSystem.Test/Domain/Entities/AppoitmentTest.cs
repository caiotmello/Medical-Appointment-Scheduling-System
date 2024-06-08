using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Test.Domain.Entities
{
    public class AppoitmentTest
    {
        [Fact(DisplayName = "[UnitTest] Appointment - Create Appoitment with success")]
        [Trait("Appointment","Appointment Validation")]
        public void Appointment_WithValidData_ShouldBeValid()
        {
            // Arrange
            var appointment = new Appointment
            {
                PatientId = "patient123",
                DoctorId = "doctor456",
                Date = DateTime.Today,
                Time = DateTime.Now.TimeOfDay,
                Status = AppoitmentStatusEnum.Scheduled
            };

            // Act
            var validationResults = ValidateModel(appointment);

            // Assert
            Assert.Empty(validationResults);
        }


        [Fact(DisplayName = "[UnitTest] Appointment - Create Appoitment without patientId")]
        [Trait("Appointment", "Appointment Validation")]
        public void Appointment_WithoutPatientId_ShouldBeInvalid()
        {
            // Arrange
            var appointment = new Appointment
            {
                DoctorId = "doctor456",
                Date = DateTime.Today,
                Time = DateTime.Now.TimeOfDay,
                Status = AppoitmentStatusEnum.Scheduled
            };

            // Act
            var validationResults = ValidateModel(appointment);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("The PatientId field is required.", validationResults[0].ErrorMessage);
        }

        [Fact(DisplayName = "[UnitTest] Appointment - Create Appoitment without doctorId")]
        [Trait("Appointment", "Appointment Validation")]
        public void Appointment_WithoutDoctorId_ShouldBeInvalid()
        {
            // Arrange
            var appointment = new Appointment
            {
                PatientId = "patient123",
                Date = DateTime.Today,
                Time = DateTime.Now.TimeOfDay,
                Status = AppoitmentStatusEnum.Scheduled
            };

            // Act
            var validationResults = ValidateModel(appointment);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("The DoctorId field is required.", validationResults[0].ErrorMessage);
        }

        [Fact(DisplayName = "[UnitTest] Appointment - Create Appoitment without date")]
        [Trait("Appointment", "Appointment Validation")]
        public void Appointment_WithoutDate_ShouldBeInvalid()
        {
            // Arrange
            var appointment = new Appointment
            {
                PatientId = "patient123",
                DoctorId = "doctor456",
                Time = DateTime.Now.TimeOfDay,
                Status = AppoitmentStatusEnum.Scheduled
            };

            // Act
            var validationResults = ValidateModel(appointment);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("The Date field is required.", validationResults[0].ErrorMessage);
        }

        [Fact(DisplayName = "[UnitTest] Appointment - Create Appoitment without time")]
        [Trait("Appointment", "Appointment Validation")]
        public void Appointment_WithoutTime_ShouldBeInvalid()
        {
            // Arrange
            var appointment = new Appointment
            {
                PatientId = "patient123",
                DoctorId = "doctor456",
                Date = DateTime.Today,
                Status = AppoitmentStatusEnum.Scheduled
            };

            // Act
            var validationResults = ValidateModel(appointment);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("The Time field is required.", validationResults[0].ErrorMessage);
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
