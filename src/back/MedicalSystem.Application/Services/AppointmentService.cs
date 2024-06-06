using MedicalSystem.Application.Dtos.Appointment;
using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MedicalSystem.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<IAppointmentService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentService(IAppointmentRepository appointmentRepository, ILogger<IAppointmentService> logger, UserManager<ApplicationUser> userManager)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<AppointmentResponceDto> CreateAsync(AppointmentCreateRequestDto model)
        {

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var newAppointment = new Appointment
            {
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                Date = model.Date,
                Time = model.Time,
                Status = AppoitmentStatusEnum.Scheduled

            };

           var result = await _appointmentRepository.AddAsync(newAppointment);

            return new AppointmentResponceDto
            {
                Id = result.Id,
                PatientId = result.PatientId,
                DoctorId = result.DoctorId,
                Date = result.Date,
                Time = result.Time,
                Status = result.Status
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception();

            appointment.Status = AppoitmentStatusEnum.Cancelled;
            await _appointmentRepository.UpdateAsync(appointment);

            return true;
        }

        public async Task<IList<AppointmentResponceDto>> GetAllAsync()
        {
            var appoitments = await _appointmentRepository.GetAllAsync();

            return appoitments.Select(appoitment => new AppointmentResponceDto
            {
                Id = appoitment.Id,
                PatientId = appoitment.PatientId,
                DoctorId = appoitment.DoctorId,
                Date = appoitment.Date,
                Time = appoitment.Time,
                Status = appoitment.Status
            }).ToList();
        }

        public async Task<IList<AppointmentResponceDto>> GetByDate(DateTime startDate, DateTime endDate)
        {
            var appoitments = await _appointmentRepository.GetAppointmentByDateAsync(startDate, endDate);

            return appoitments.Select(appoitment => new AppointmentResponceDto
            {
                Id = appoitment.Id,
                PatientId = appoitment.PatientId,
                DoctorId = appoitment.DoctorId,
                Date = appoitment.Date,
                Time = appoitment.Time,
                Status = appoitment.Status
            }).ToList();
        }
    
        public async Task<IList<AppointmentResponceDto>> GetByDoctorAsync(string doctorId)
        {
            var doctor = await _userManager.FindByIdAsync(doctorId);
            if (doctor == null)
                throw new Exception("Doctor doesn't exist!");

            var appoitments = await _appointmentRepository.GetAppointmentByDoctor((Doctor)doctor);
            return appoitments.Select(appoitment => new AppointmentResponceDto
            {
                Id = appoitment.Id,
                PatientId = appoitment.PatientId,
                DoctorId = appoitment.DoctorId,
                Date = appoitment.Date,
                Time = appoitment.Time,
                Status = appoitment.Status
            }).ToList();

        }

        public async Task<AppointmentResponceDto> GetByIdAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            
            return new AppointmentResponceDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                Date = appointment.Date,
                Time = appointment.Time,
                Status = appointment.Status
            };
        }

        public async Task<IList<AppointmentResponceDto>> GetByPatientAsync(string patientId)
        {
            var patient = await _userManager.FindByIdAsync(patientId);
            if (patient == null)
                throw new Exception("Doctor doesn't exist!");

            var appoitments = await _appointmentRepository.GetAppointmentByPatient((Patient)patient);
            return appoitments.Select(appoitment => new AppointmentResponceDto
            {
                Id = appoitment.Id,
                PatientId = appoitment.PatientId,
                DoctorId = appoitment.DoctorId,
                Date = appoitment.Date,
                Time = appoitment.Time,
                Status = appoitment.Status
            }).ToList();
        }

        public async Task<AppointmentResponceDto> UpdateAsync(AppointmentUpdateRequestDto model)
        {
            if (model == null)
                throw new ArgumentNullException();

            var appointment = await _appointmentRepository.GetByIdAsync(model.Id);
            if (appointment == null)
                throw new Exception();

            appointment.Date = model.Date;
            appointment.Time = model.Time;
            appointment.Status = model.Status;

            await _appointmentRepository.UpdateAsync(appointment);

            return new AppointmentResponceDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                Date = appointment.Date,
                Time = appointment.Time,
                Status = appointment.Status
            };
        }
    
        public async Task<IList<AppointmentWithEmailResponseDto>> GetAppointmentByDateAsync(DateTime date)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentByDateAsync(date);
                var appointmentsDtos = new List<AppointmentWithEmailResponseDto>();

                foreach (var appointment in appointments)
                {
                    var patient = await _userManager.FindByIdAsync(appointment.PatientId);
                    var doctor = await _userManager.FindByIdAsync(appointment.DoctorId);

                    var appointmentDto = new AppointmentWithEmailResponseDto
                    {
                        Patient = new ApplicationUser
                        {
                            FirstName = patient.FirstName,
                            LastName = patient.LastName,
                            Email = patient.Email,
                        },
                        Doctor = new ApplicationUser
                        {
                            FirstName = doctor.FirstName,
                            LastName = doctor.LastName,
                            Email = doctor.Email,
                        },
                        Date = appointment.Date,
                        Time = appointment.Time,
                        Status = appointment.Status
                    };


                    appointmentsDtos.Add(appointmentDto);
                }
                return appointmentsDtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve appoitments!!",ex);
            }
  
        }
    }
}
