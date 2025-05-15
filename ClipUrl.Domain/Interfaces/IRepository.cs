using System.Linq.Expressions;

namespace ClipUrl.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdAsync(Guid id);

        Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes);

        Task<T?> GetEntityAsync(Expression<Func<T, bool>> predicate);

        Task<T?> GetEntityWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetEntitiesAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetEntitiesWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        void Delete(T entity);

        Task SaveChangesAsync();
    }
}
