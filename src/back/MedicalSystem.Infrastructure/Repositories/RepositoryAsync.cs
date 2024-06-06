using MedicalSystem.Application.Interfaces.Repositories;
using MedicalSystem.Domain.Entities;
using MedicalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace MedicalSystem.Infrastructure.Repositories
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : EntityBase
    {
        protected readonly AppDbContext _dbContext;
        private readonly ILogger<IRepositoryAsync<T>> _logger;

        public RepositoryAsync(AppDbContext dbContext, ILogger<IRepositoryAsync<T>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                var result = await _dbContext.Set<T>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Adding entity - Exception] - {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<int> Count() => await _dbContext.Set<T>().CountAsync();

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>   await _dbContext.Set<T>().ToListAsync();

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate) => await _dbContext.Set<T>().Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> GetAsync(int offset, int limit)
        {
            var result = await _dbContext.Set<T>()
             .OrderBy(e => e.Id)
             .Skip(offset)
             .Take(limit)
             .ToListAsync();
            return result;
        }

        public async Task<T?> GetByIdAsync(Guid id) => await _dbContext.Set<T>().FindAsync(id);


        public async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
