using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RoutingService.Domain.Specifications.Base
{
    /// <summary>
    /// Specification pattern interface
    /// Encapsulates query logic in reusable, testable objects
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface ISpecification<T>
    {
        // Filtering
        Expression<Func<T, bool>>? Criteria { get; }
        List<Expression<Func<T, bool>>> AndCriteria { get; }

        // Includes for eager loading
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }

        // Sorting
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }

        // Paging
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }

        // Tracking
        bool AsNoTracking { get; }
    }
}