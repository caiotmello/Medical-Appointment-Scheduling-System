using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Test.Domain.Entities
{
    public class PatientTest
    {
        [Fact(DisplayName = "[UnitTest] Patient - Create Patient with success")]
        [Trait("Patient", "Patient Validation")]
        public void Patient_WithValidData_ShouldBeValid()
        {
            // Arrange
            var patient = new Patient
            {
                UserName = "teste@email.com",
                CPF = "123.456.789-09",
                FirstName = "Patient",
                LastName = "Last",
                PhoneNumber = "98883898387",
                Email = "teste@email.com",
                Status = UserStatusEnum.Active,
            };

            // Act
            var validationResults = ValidateModel(patient);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact(DisplayName = "[UnitTest] Patient - Create Patient without CPF")]
        [Trait("Patient", "Patient Validation")]
        public void Patient_WithoutCPF_ShouldBeInvalid()
        {
            // Arrange
            var patient = new Patient
            {
                UserName = "teste@email.com",
                FirstName = "Patient",
                LastName = "Last",
                PhoneNumber = "98883898387",
                Email = "teste@email.com",
                Status = UserStatusEnum.Active,
            };

            // Act
            var validationResults = ValidateModel(patient);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("The CPF field is required.", validationResults[0].ErrorMessage);
        }

        [Fact(DisplayName = "[UnitTest] Patient - Create Patient with success")]
        [Trait("Patient", "Patient Validation")]
        public void Patient_WithInvalidCPF_ShouldBeInvalid()
        {
            // Arrange
            var patient = new Patient
            {
                UserName = "teste@email.com",
                CPF = "000.000.000-00",
                FirstName = "Patient",
                LastName = "Last",
                PhoneNumber = "98883898387",
                Email = "teste@email.com",
                Status = UserStatusEnum.Active,
            };

            // Act
            var validationResults = ValidateModel(patient);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("Invalid CPF.", validationResults[0].ErrorMessage);
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
