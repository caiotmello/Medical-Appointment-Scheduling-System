using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MedicalSystem.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly IdentityDataContext _identityDataContext;

        public DoctorRepository(IdentityDataContext identityDataContext)
        {
            _identityDataContext = identityDataContext;
        }

        public async Task<IEnumerable<Doctor>> GetAsync(Expression<Func<Doctor, bool>> predicate) =>
            await _identityDataContext.Set<Doctor>().Where(predicate).ToListAsync();

        public async Task<Doctor> GetByIdAsync(string id) =>
            await _identityDataContext.Set<Doctor>().FindAsync(id);

        public async Task UpdateAsync(Doctor entity)
        {
            _identityDataContext.Set<Doctor>().Attach(entity);
            _identityDataContext.Entry(entity).State = EntityState.Modified;
            await _identityDataContext.SaveChangesAsync();
        }
    }
}
