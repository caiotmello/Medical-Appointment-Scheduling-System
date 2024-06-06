using MedicalSystem.Application.Dtos.Doctor;
using MedicalSystem.Domain.Enumerations;

namespace MedicalSystem.Application.Interfaces.Services
{
    public interface IDoctorService
    {
        Task<IList<DoctorResponceDto>> GetAllAsync();
        Task<DoctorResponceDto> GetByIdAsync(string id);
        Task<IList<DoctorResponceDto>> GetBySpeciality(SpecialityEnum speciality);
        Task<DoctorResponceDto> UpdateAsync(DoctorUpdateRequestDto model);
    }
}
