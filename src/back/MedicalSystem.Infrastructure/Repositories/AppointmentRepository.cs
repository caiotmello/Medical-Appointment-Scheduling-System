using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MedicalSystem.Infrastructure.Repositories
{
    public class AppointmentRepository : RepositoryAsync<Appointment>, IAppointmentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<IAppointmentRepository> _logger;
        public AppointmentRepository(AppDbContext dbContext, ILogger<IAppointmentRepository> logger) : base(dbContext,logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IList<Appointment>> GetAppointmentByDoctor(Doctor doctor)
        {

            var appointments = await _dbContext.Set<Appointment>()
                .Where(appointment => appointment.DoctorId == doctor.Id)
                .OrderByDescending(appointment => appointment.Date)
                .ThenByDescending(appointment => appointment.Time)
                .ToListAsync();

            return appointments;
        }

        public async Task<IList<Appointment>> GetAppointmentByPatient(Patient patient)
        {        
            var appointments = await _dbContext.Set<Appointment>()
               .Where(appointment => appointment.PatientId == patient.Id)
               .OrderByDescending(appointment => appointment.Date)
               .ThenByDescending(appointment => appointment.Time)
               .ToListAsync();

            return appointments;
        }
        public async Task<IList<Appointment>> GetAppointmentByDateAsync(DateTime start, DateTime end)
        {
            return await _dbContext.Set<Appointment>()
                            .Where(entity => entity.CreatedAt >= start && entity.CreatedAt <= end)
                            .OrderByDescending(e => e.CreatedAt)
                            .ToListAsync();
        }

        public async Task<IList<Appointment>> GetAppointmentByDateAsync(DateTime date)
        {
            return await _dbContext.Set<Appointment>()
                    .Where(a => a.Date.Date == date.Date)
                    .ToListAsync();
        }
    }
}
