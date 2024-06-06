namespace MedicalSystem.Application.Dtos.Patient
{
    public class PatientUpdateRequestDto
    {
        public string Id { get; set; }
        public string CPF { get; set; }
        public DateTime BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
