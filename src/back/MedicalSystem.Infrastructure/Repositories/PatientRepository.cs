using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MedicalSystem.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IdentityDataContext _identityDataContext;

        public PatientRepository(IdentityDataContext identityDataContext)
        {
            _identityDataContext = identityDataContext;
        }

        public async Task<IEnumerable<Patient>> GetAsync(Expression<Func<Patient, bool>> predicate) =>
            await _identityDataContext.Set<Patient>().Where(predicate).ToListAsync();

        public async Task<Patient> GetByIdAsync(string id) =>
            await _identityDataContext.Set<Patient>().FindAsync(id);

        public async Task UpdateAsync(Patient entity)
        {
            _identityDataContext.Set<Patient>().Attach(entity);
            _identityDataContext.Entry(entity).State = EntityState.Modified;
            await _identityDataContext.SaveChangesAsync();
        }
    }
}
