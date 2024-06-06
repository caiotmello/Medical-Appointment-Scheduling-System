using MedicalSystem.Application.Dtos.Appointment;

namespace MedicalSystem.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentResponceDto> CreateAsync(AppointmentCreateRequestDto appointment);
        Task<AppointmentResponceDto> UpdateAsync(AppointmentUpdateRequestDto appointment);
        Task<bool> DeleteAsync(Guid id);
        Task<AppointmentResponceDto> GetByIdAsync(Guid id);
        Task<IList<AppointmentResponceDto>> GetAllAsync();
        Task<IList<AppointmentResponceDto>> GetByDoctorAsync(string doctorId);
        Task<IList<AppointmentResponceDto>> GetByPatientAsync(string patientId);
        Task<IList<AppointmentResponceDto>> GetByDate(DateTime startDate, DateTime endDate);
        Task<IList<AppointmentWithEmailResponseDto>> GetAppointmentByDateAsync(DateTime date);


        //Create
        //Edit (Admin, Patient)
        //Delete
        //GetAvailableAppointments (docID, Date) -> ver se vai ser facil de implemntar
        //GetAppointmentByMedico
        //GetAppointmentByPatient
        //GetAppoitmentByDate
        //GetAppoitmentById
    }
}
