using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Specifications.Base;
using System.Linq;

namespace RoutingService.Dal.Repositories
{
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(
            IQueryable<T> inputQuery,
            ISpecification<T> specification)
        {
            var query = inputQuery;

            if (specification.AsNoTracking)
            {
                query = query.AsNoTracking();
            }

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            foreach (var criteria in specification.AndCriteria)
            {
                query = query.Where(criteria);
            }

            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            query = specification.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));

            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                            .Take(specification.Take);
            }

            return query;
        }
    }
}