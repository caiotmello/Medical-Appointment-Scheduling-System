using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;

namespace MedicalSystem.Application.Dtos.Appointment
{
    public class AppointmentWithEmailResponseDto
    {
        public Guid Id { get; set; }
        public ApplicationUser Patient { get; set; }
        public ApplicationUser Doctor { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public AppoitmentStatusEnum Status { get; set; }
    }
}
