using MedicalSystem.Domain.Enumerations;
using MedicalSystem.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Domain.Entities
{
    public class Doctor : ApplicationUser
    {
        [Required]
        [CRM]
        public string CRM { get; set; }
        public SpecialityEnum? Speciality { get; set; }

    }
}
