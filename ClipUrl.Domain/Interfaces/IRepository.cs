using System.Linq.Expressions;

namespace ClipUrl.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdAsync(Guid id);

        Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes);

        Task AddAsync(T entity);

        void UpdateAsync(T entity);

        void DeleteAsync(T entity);

        Task SaveChangesAsync();
    }
}
