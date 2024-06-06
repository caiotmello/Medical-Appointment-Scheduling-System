using MedicalSystem.Application.Dtos.Patient;

namespace MedicalSystem.Application.Interfaces.Services
{
    public interface IPatientService
    {
        Task<IList<PatientResponceDto>> GetAllAsync();
        Task<PatientResponceDto> GetByIdAsync(string id);
        Task<PatientResponceDto> UpdateAsync(PatientUpdateRequestDto model);
    }
}
