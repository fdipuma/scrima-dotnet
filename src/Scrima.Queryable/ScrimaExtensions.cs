using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Scrima.Core.Query;

namespace Scrima.Queryable
{
    public static partial class ScrimaExtensions
    {
        public static QueryResult<T> ToQueryResult<T>(this IQueryable<T> source, QueryOptions queryOptions, Expression<Func<T, string, bool>> searchPredicate = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (queryOptions == null) throw new ArgumentNullException(nameof(queryOptions));

            // The order of applying the items to the IQueryable is important
            // 1. apply query and order
            source = ApplyQuery(source, queryOptions, searchPredicate);

            // 2. optionally get the count of unfiltered items
            long? count = null;
            if (queryOptions.ShowCount) count = source.LongCount();

            // 3. apply paging on the sorted and filtered result.
            source = source.Paginate(queryOptions);

            // 4. materialize results
            var result = source.ToList();

            return new QueryResult<T>(result, count);
        }

        /// <summary>
        /// This method is used by friendly assemblies to implement the async version. This is usually done
        /// because async counterpars (ToListAsync, CountAsync, etc) are available only on specific frameworks
        /// (e.g. Entity Framework, EF Core, Hibernate). 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="queryOptions"></param>
        /// <param name="toListAsync"></param>
        /// <param name="longCountAsync"></param>
        /// <param name="searchPredicate"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<QueryResult<T>> ToQueryResultInternalAsync<T>(IQueryable<T> source, 
            QueryOptions queryOptions,
            Func<IQueryable<T>, CancellationToken, Task<List<T>>> toListAsync,
            Func<IQueryable<T>, CancellationToken, Task<long>> longCountAsync,
            Expression<Func<T, string, bool>> searchPredicate,
            CancellationToken cancellationToken)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (queryOptions == null) throw new ArgumentNullException(nameof(queryOptions));

            // The order of applying the items to the IQueryable is important
            // 1. apply query and order
            source = ApplyQuery(source, queryOptions, searchPredicate);

            // 2. optionally get the count of unfiltered items
            long? count = null;
            if (queryOptions.ShowCount) count = await longCountAsync.Invoke(source, cancellationToken);

            // 3. apply paging on the sorted and filtered result.
            source = source.Paginate(queryOptions);

            // 4. materialize results
            var result = await toListAsync.Invoke(source, cancellationToken);

            return new QueryResult<T>(result, count);
        }

        private static IQueryable<TSource> ApplyQuery<TSource>(IQueryable<TSource> source, QueryOptions queryOptions,
            Expression<Func<TSource, string, bool>> searchPredicate)
        {
            // 1. sort to have the correct order for filtering and limiting
            source = source.OrderBy(queryOptions.OrderBy);

            // 2. filter the items according to the user input
            source = source.Where(queryOptions.Filter);

            // 3. handle the "search" parameter
            return source.Search(queryOptions, searchPredicate);
        }

        private static IQueryable<TSource> Search<TSource>(this IQueryable<TSource> source, QueryOptions queryOptions, Expression<Func<TSource, string, bool>> searchPredicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (queryOptions == null) throw new ArgumentNullException(nameof(queryOptions));

            if (queryOptions.Search is null || searchPredicate is null)
                return source;
            
            // we are transforming a generic expression with two arguments into
            // a single argument expression that uses a constant value instead of a parameter
            // e.g.:
            // original:
            //                (element, searchText) => element.MyValue.Contains(searchText)
            // transformed:
            //                var x = "my current search text";
            //                element => element.MyValue.Contains(x);
            var predicate = ExpressionHelper.BindSecondArgument(searchPredicate, queryOptions.Search);
            return source.Where(predicate);
        }

        public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> source,
            QueryOptions queryOptions)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (queryOptions == null) throw new ArgumentNullException(nameof(queryOptions));

            source = source.Skip((int)(queryOptions.Skip ?? 0));

            var top = queryOptions.Top;
            if (top.HasValue)
            {
                source = source.Take((int)top);
            }

            return source;
        }
    }
}
