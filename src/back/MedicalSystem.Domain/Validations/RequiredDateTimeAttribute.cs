using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Domain.Validations
{
    public class RequiredDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime == DateTime.MinValue)
                {
                    return new ValidationResult("The Date field is required.");
                }
            }
            else
            {
                return new ValidationResult("The provided value is not a valid date.");
            }

            return ValidationResult.Success;
        }
    }
}
