using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MedicalSystem.Domain.Validations
{
    public class CPFAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // CPF pode ser null, a menos que [Required] seja usado
            }

            var cpf = value as string;

            if (!IsValidCpf(cpf))
            {
                return new ValidationResult("Invalid CPF.");
            }

            return ValidationResult.Success;
        }

        private bool IsValidCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return false;
            }

            cpf = Regex.Replace(cpf, "[^0-9]", string.Empty);

            if (cpf.Length != 11)
            {
                return false;
            }

            var invalidCpfs = new[]
            {
            "00000000000", "11111111111", "22222222222", "33333333333",
            "44444444444", "55555555555", "66666666666", "77777777777",
            "88888888888", "99999999999"
        };

            if (invalidCpfs.Contains(cpf))
            {
                return false;
            }

            var cpfArray = cpf.Select(x => int.Parse(x.ToString())).ToArray();

            for (var i = 9; i < 11; i++)
            {
                var sum = 0;
                for (var j = 0; j < i; j++)
                {
                    sum += cpfArray[j] * (i + 1 - j);
                }

                var remainder = (sum * 10) % 11;
                if (remainder == 10 || remainder == 11)
                {
                    remainder = 0;
                }

                if (remainder != cpfArray[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
