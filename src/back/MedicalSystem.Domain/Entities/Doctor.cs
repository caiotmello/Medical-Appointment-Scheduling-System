using MedicalSystem.Domain.Enumerations;

namespace MedicalSystem.Domain.Entities
{
    public class Doctor : ApplicationUser
    {
        public string? CRM { get; set; }
        public SpecialityEnum? Speciality { get; set; }
    }
}
