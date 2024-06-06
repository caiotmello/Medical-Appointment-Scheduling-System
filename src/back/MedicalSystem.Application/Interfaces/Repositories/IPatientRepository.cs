using MedicalSystem.Domain.Entities;
using System.Linq.Expressions;

namespace MedicalSystem.Application.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAsync(Expression<Func<Patient, bool>> predicate);
        Task<Patient> GetByIdAsync(string id);
        Task UpdateAsync(Patient entity);

    }
}
