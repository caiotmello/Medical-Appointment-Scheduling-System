using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Domain.Validations
{
    public class RequiredTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is TimeSpan timeSpan)
            {
                if (timeSpan == TimeSpan.Zero)
                {
                    return new ValidationResult("The Time field is required.");
                }
            }
            else if (value is DateTime dateTime)
            {
                if (dateTime.TimeOfDay == TimeSpan.Zero)
                {
                    return new ValidationResult("The Time field is required.");
                }
            }

            return ValidationResult.Success;
        }

    }
}
