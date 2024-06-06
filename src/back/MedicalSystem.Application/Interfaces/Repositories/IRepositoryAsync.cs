using System.Linq.Expressions;

namespace MedicalSystem.Application.Interfaces.Repositories
{
    public interface IRepositoryAsync<T> where T: class
    {
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync (T entity);
        Task<T?> GetByIdAsync (Guid id);
        Task<IEnumerable<T>> GetAllAsync ();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAsync(int offset, int limit);
        Task<int> Count();
    }
}
