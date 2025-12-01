using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Specifications.Base;
using System.Linq;

namespace RoutingService.Infrastructure.Repositories
{
    /// <summary>
    /// Evaluates specifications and applies them to IQueryable
    /// Translates specification objects into EF Core query operations
    /// </summary>
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(
            IQueryable<T> inputQuery,
            ISpecification<T> specification)
        {
            var query = inputQuery;

            // Apply no tracking if specified
            if (specification.AsNoTracking)
            {
                query = query.AsNoTracking();
            }

            // Apply criteria (WHERE clause)
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Apply additional AND criteria
            foreach (var criteria in specification.AndCriteria)
            {
                query = query.Where(criteria);
            }

            // Apply includes for eager loading
            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            // Apply string-based includes
            query = specification.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply paging (must be after ordering)
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                            .Take(specification.Take);
            }

            return query;
        }
    }
}