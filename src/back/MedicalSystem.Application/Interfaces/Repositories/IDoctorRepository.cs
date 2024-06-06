using MedicalSystem.Domain.Entities;
using System.Linq.Expressions;

namespace MedicalSystem.Application.Interfaces.Repositories
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAsync(Expression<Func<Doctor, bool>> predicate);
        Task<Doctor> GetByIdAsync(string id);
        Task UpdateAsync(Doctor entity);

    }
}
