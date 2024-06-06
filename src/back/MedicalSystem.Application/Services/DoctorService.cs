using MedicalSystem.Application.Dtos.Doctor;
using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace MedicalSystem.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ILogger<DoctorService> _logger;
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(ILogger<DoctorService> logger, IDoctorRepository doctorRepository)
        {
            _logger = logger;
            _doctorRepository = doctorRepository;
        }

        public async Task<IList<DoctorResponceDto>> GetAllAsync()
        {
            var doctors = await _doctorRepository.GetAsync(doctor => doctor.Status != UserStatusEnum.Inactive);

            return doctors.Select(doctor => new DoctorResponceDto
            {
                Id = doctor.Id,
                Username = doctor.UserName,
                CRM = doctor.CRM,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                PhoneNumber = doctor.PhoneNumber,
                Email = doctor.Email,
                Status = doctor.Status
            }).ToList();

        }

        public async Task<DoctorResponceDto> GetByIdAsync(string id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);

            if (doctor is null)
                throw new ArgumentException("Doctor not found");
            
            return new DoctorResponceDto
            {
                Id = doctor.Id,
                Username = doctor.UserName,
                CRM = doctor.CRM,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                PhoneNumber = doctor.PhoneNumber,
                Email = doctor.Email,
                Status = doctor.Status
            };
        }

        public async Task<IList<DoctorResponceDto>> GetBySpeciality(SpecialityEnum speciality)
        {
            var doctors = await _doctorRepository.GetAsync(doctor => doctor.Speciality == speciality);

            return doctors.Select(doctor => new DoctorResponceDto
            {
                Id = doctor.Id,
                Username = doctor.UserName,
                CRM = doctor.CRM,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                PhoneNumber = doctor.PhoneNumber,
                Email = doctor.Email,
                Status = doctor.Status
            }).ToList();
        }

        public async Task<DoctorResponceDto> UpdateAsync(DoctorUpdateRequestDto model)
        {
            if (model == null)
                throw new ArgumentNullException();

            var doctor = await _doctorRepository.GetByIdAsync(model.Id);
            if (doctor == null)
                throw new Exception();

            doctor.CRM = model.CRM;
            doctor.FirstName = model.FirstName;
            doctor.LastName = model.LastName;
            doctor.PhoneNumber = model.PhoneNumber;

            await _doctorRepository.UpdateAsync(doctor);

            return new DoctorResponceDto
            {
                Id = doctor.Id,
                Username = doctor.UserName,
                CRM = doctor.CRM,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                PhoneNumber = doctor.PhoneNumber,
                Email = doctor.Email,
                Status = doctor.Status
            };
        }
    }
}
