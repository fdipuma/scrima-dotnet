using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scrima.Core.Query;
using Scrima.Queryable;

namespace Scrima.EntityFrameworkCore
{
    public static class EntityFrameworkScrimaExtensions
    {
        public static async Task<ScrimaQueryResult<T>> ToQueryResultAsync<T>(this IQueryable<T> source,
            ScrimaQueryOptions<T> scrimaQueryOptions, Expression<Func<T, string, bool>> searchPredicate = null,
            CancellationToken cancellationToken = default)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (scrimaQueryOptions == null) throw new ArgumentNullException(nameof(scrimaQueryOptions));

            return await ScrimaExtensions.ToQueryResultInternalAsync(
                source,
                scrimaQueryOptions,
                EntityFrameworkQueryableExtensions.ToListAsync,
                EntityFrameworkQueryableExtensions.LongCountAsync,
                searchPredicate,
                cancellationToken
            );
        }
    }
}
