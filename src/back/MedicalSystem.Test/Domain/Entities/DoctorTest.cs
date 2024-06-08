using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Test.Domain.Entities
{
    public class DoctorTest
    {
        [Fact(DisplayName = "[UnitTest] Doctor - Create Doctor with success")]
        [Trait("Doctor", "Doctor Validation")]
        public void Doctor_WithValidData_ShouldBeValid()
        {
            // Arrange
            var doctor = new Doctor
            {
                UserName = "teste@email.com",
                CRM = "411234",
                FirstName = "Doctor",
                LastName = "Last",
                PhoneNumber = "98883898387",
                Email = "teste@email.com",
                Status = UserStatusEnum.Active,
            };

            // Act
            var validationResults = ValidateModel(doctor);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact(DisplayName = "[UnitTest] Doctor - Create Doctor without CRM")]
        [Trait("Doctor", "Doctor Validation")]
        public void Doctor_WithoutCRM_ShouldBeInvalid()
        {
            // Arrange
            var doctor = new Doctor
            {
                UserName = "teste@email.com",
                FirstName = "Doctor",
                LastName = "Last",
                PhoneNumber = "98883898387",
                Email = "teste@email.com",
                Status = UserStatusEnum.Active,
            };

            // Act
            var validationResults = ValidateModel(doctor);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("The CRM field is required.", validationResults[0].ErrorMessage);
        }

        [Fact(DisplayName = "[UnitTest] Doctor - Create Doctor with with invalid CRM")]
        [Trait("Doctor", "Doctor Validation")]
        public void Doctor_WithInvalidCRM_ShouldBeInvalid()
        {
            // Arrange
            var doctor = new Doctor
            {
                UserName = "teste@email.com",
                CRM = "411d34",
                FirstName = "Doctor",
                LastName = "Last",
                PhoneNumber = "98883898387",
                Email = "teste@email.com",
                Status = UserStatusEnum.Active,
            };

            // Act
            var validationResults = ValidateModel(doctor);

            // Assert
            Assert.Single(validationResults);
            Assert.Equal("Invalid CRM.", validationResults[0].ErrorMessage);
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
