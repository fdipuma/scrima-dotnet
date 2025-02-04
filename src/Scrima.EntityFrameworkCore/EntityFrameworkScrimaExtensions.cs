using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scrima.Core.Query;
using Scrima.Queryable;

namespace Scrima.EntityFrameworkCore;

public static class EntityFrameworkScrimaExtensions
{
    public static async Task<QueryResult<T>> ToQueryResultAsync<T>(this IQueryable<T> source,
        QueryOptions queryOptions, Expression<Func<T, string, bool>> searchPredicate = null,
        CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (queryOptions == null) throw new ArgumentNullException(nameof(queryOptions));

        return await ScrimaExtensions.ToQueryResultInternalAsync(
            source,
            queryOptions,
            EntityFrameworkQueryableExtensions.ToListAsync,
            EntityFrameworkQueryableExtensions.LongCountAsync,
            searchPredicate,
            cancellationToken
        );
    }
    
    public static IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IQueryable<T> source,
        QueryOptions queryOptions, Expression<Func<T, string, bool>> searchPredicate = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (queryOptions == null) throw new ArgumentNullException(nameof(queryOptions));

        return ScrimaExtensions.AsAsyncEnumerableInternal(
            source,
            queryOptions,
            EntityFrameworkQueryableExtensions.AsAsyncEnumerable,
            searchPredicate
        );
    }
}
