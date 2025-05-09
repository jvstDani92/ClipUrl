using ClipUrl.Domain.Interfaces;
using ClipUrl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClipUrl.Infrastructure.Repositories
{
    public class ClipUrlDbRepository<T> : IRepository<T> where T : class
    {
        private readonly ClipUrlDbContext _context;
        private readonly DbSet<T> _entities;

        public ClipUrlDbRepository(ClipUrlDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _entities.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            var query = _entities.AsNoTracking();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> GetEntitiesAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetEntitiesWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = _entities.AsNoTracking();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T?> GetEntityAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetEntityWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = _entities.AsNoTracking();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await _entities.FirstOrDefaultAsync(predicate);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
