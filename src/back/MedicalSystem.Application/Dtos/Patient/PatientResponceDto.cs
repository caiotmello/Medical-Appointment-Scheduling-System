using MedicalSystem.Domain.Enumerations;

namespace MedicalSystem.Application.Dtos.Patient
{
    public class PatientResponceDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string CPF { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public UserStatusEnum Status { get; set; }
    }
}
