using MedicalSystem.Domain.Entities;
using System.ComponentModel;

namespace MedicalSystem.Application.Interfaces.Repositories
{
    public interface IAppointmentRepository: IRepositoryAsync<Appointment>
    {
        Task<IList<Appointment>> GetAppointmentByDoctor(Doctor doctor);
        Task<IList<Appointment>> GetAppointmentByPatient(Patient patient);
        Task<IList<Appointment>> GetAppointmentByDateAsync(DateTime start, DateTime end);
        Task<IList<Appointment>> GetAppointmentByDateAsync(DateTime date);

    }
}
