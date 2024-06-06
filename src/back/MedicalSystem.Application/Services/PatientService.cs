using MedicalSystem.Application.Dtos.Patient;
using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace MedicalSystem.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly ILogger<PatientService> _logger;
        private readonly IPatientRepository _PatientRepository;

        public PatientService(ILogger<PatientService> logger, IPatientRepository PatientRepository)
        {
            _logger = logger;
            _PatientRepository = PatientRepository;
        }

        public async Task<IList<PatientResponceDto>> GetAllAsync()
        {
            var Patients = await _PatientRepository.GetAsync(Patient => Patient.Status != UserStatusEnum.Inactive);

            return Patients.Select(Patient => new PatientResponceDto
            {
                Id = Patient.Id,
                Username = Patient.UserName,
                CPF = Patient.CPF,
                BirthDate = Patient.BirthDate,
                FirstName = Patient.FirstName,
                LastName = Patient.LastName,
                PhoneNumber = Patient.PhoneNumber,
                Email = Patient.Email,
                Status = Patient.Status
            }).ToList();

        }

        public async Task<PatientResponceDto> GetByIdAsync(string id)
        {
            var Patient = await _PatientRepository.GetByIdAsync(id);

            if (Patient is null)
                throw new ArgumentException("Patient not found");
            
            return new PatientResponceDto
            {
                Id = Patient.Id,
                Username = Patient.UserName,
                CPF = Patient.CPF,
                BirthDate = Patient.BirthDate,
                FirstName = Patient.FirstName,
                LastName = Patient.LastName,
                PhoneNumber = Patient.PhoneNumber,
                Email = Patient.Email,
                Status = Patient.Status
            };
        }

        public async Task<PatientResponceDto> UpdateAsync(PatientUpdateRequestDto model)
        {
            if (model == null)
                throw new ArgumentNullException();

            var Patient = await _PatientRepository.GetByIdAsync(model.Id);
            if (Patient == null)
                throw new Exception();

            Patient.CPF = model.CPF;
            Patient.FirstName = model.FirstName;
            Patient.LastName = model.LastName;
            Patient.PhoneNumber = model.PhoneNumber;

            await _PatientRepository.UpdateAsync(Patient);

            return new PatientResponceDto
            {
                Id = Patient.Id,
                Username = Patient.UserName,
                CPF = Patient.CPF,
                BirthDate = Patient.BirthDate,
                FirstName = Patient.FirstName,
                LastName = Patient.LastName,
                PhoneNumber = Patient.PhoneNumber,
                Email = Patient.Email,
                Status = Patient.Status
            };
        }
    }
}
