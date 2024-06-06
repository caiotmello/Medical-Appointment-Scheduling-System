using MedicalSystem.Domain.Enumerations;

namespace MedicalSystem.Application.Dtos.Appointment
{
    public class AppointmentResponceDto
    {
        public Guid Id { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public AppoitmentStatusEnum Status { get; set; }
    }
}
