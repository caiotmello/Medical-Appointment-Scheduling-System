using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MedicalSystem.Domain.Validations
{
    public class CRMAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // CRM pode ser null ou vazio, a menos que [Required] seja usado
            }

            var crm = value.ToString();

            if (!IsValidCrm(crm))
            {
                return new ValidationResult("Invalid CRM.");
            }

            return ValidationResult.Success;
        }

        private bool IsValidCrm(string crm)
        {
            if (string.IsNullOrWhiteSpace(crm))
            {
                return false;
            }

            crm = Regex.Replace(crm, "[^0-9]", string.Empty);

            if (crm.Length != 6)
            {
                return false;
            }

            return true;
        }

    }
}
