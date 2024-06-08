using MedicalSystem.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Domain.Entities
{
    public class Patient : ApplicationUser
    {
        [Required]
        [CPF]
        public string CPF {  get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
