using Microsoft.EntityFrameworkCore;
using RoutingService.Dal.Data;
using RoutingService.Domain.Repositories;
using RoutingService.Domain.Specifications.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RoutingService.Dal.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly RoutingDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(RoutingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();

            return await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<IEnumerable<T>> FindWithSpecificationAsync(ISpecification<T> specification)
        {
            var query = SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), specification);
            return await query.ToListAsync();
        }

        public virtual async Task<T?> FindOneWithSpecificationAsync(ISpecification<T> specification)
        {
            var query = SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), specification);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<int> CountWithSpecificationAsync(ISpecification<T> specification)
        {
            var query = SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), specification);
            return await query.CountAsync();
        }
    }
}