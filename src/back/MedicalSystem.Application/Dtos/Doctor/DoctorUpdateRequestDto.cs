namespace MedicalSystem.Application.Dtos.Doctor
{
    public class DoctorUpdateRequestDto
    {
        public string Id { get; set; }
        public string CRM { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
