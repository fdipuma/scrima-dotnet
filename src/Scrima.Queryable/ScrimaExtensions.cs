using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Scrima.Core.Query;

[assembly:InternalsVisibleTo("Scrima.EntityFrameworkCore")]

namespace Scrima.Queryable
{
    public static partial class ScrimaExtensions
    {
        public static ScrimaQueryResult<T> ToQueryResult<T>(this IQueryable<T> source, ScrimaQueryOptions<T> scrimaQueryOptions, Expression<Func<T, string, bool>> searchPredicate = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (scrimaQueryOptions == null) throw new ArgumentNullException(nameof(scrimaQueryOptions));

            // The order of applying the items to the IQueryable is important
            // 1. apply query and order
            source = ApplyQuery(source, scrimaQueryOptions, searchPredicate);

            // 2. optionally get the count of unfiltered items
            long? count = null;
            if (scrimaQueryOptions.ShowCount) count = source.LongCount();

            // 3. apply paging on the sorted and filtered result.
            source = source.Paginate(scrimaQueryOptions);

            // 4. materialize results
            var result = source.ToList();

            return new ScrimaQueryResult<T>(scrimaQueryOptions, result, count);
        }

        /// <summary>
        /// This method is used by friendly assemblies to implement the async version. This is usually done
        /// because async counterpars (ToListAsync, CountAsync, etc) are available only on specific frameworks
        /// (e.g. Entity Framework, EF Core, Hibernate). 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="scrimaQueryOptions"></param>
        /// <param name="toListAsync"></param>
        /// <param name="longCountAsync"></param>
        /// <param name="searchPredicate"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static async Task<ScrimaQueryResult<T>> ToQueryResultInternalAsync<T>(IQueryable<T> source, 
            ScrimaQueryOptions<T> scrimaQueryOptions,
            Func<IQueryable<T>, CancellationToken, Task<List<T>>> toListAsync,
            Func<IQueryable<T>, CancellationToken, Task<long>> longCountAsync,
            Expression<Func<T, string, bool>> searchPredicate,
            CancellationToken cancellationToken)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (scrimaQueryOptions == null) throw new ArgumentNullException(nameof(scrimaQueryOptions));

            // The order of applying the items to the IQueryable is important
            // 1. apply query and order
            source = ApplyQuery(source, scrimaQueryOptions, searchPredicate);

            // 2. optionally get the count of unfiltered items
            long? count = null;
            if (scrimaQueryOptions.ShowCount) count = await longCountAsync.Invoke(source, cancellationToken);

            // 3. apply paging on the sorted and filtered result.
            source = source.Paginate(scrimaQueryOptions);

            // 4. materialize results
            var result = await toListAsync.Invoke(source, cancellationToken);

            return new ScrimaQueryResult<T>(scrimaQueryOptions, result, count);
        }

        private static IQueryable<TSource> ApplyQuery<TSource>(IQueryable<TSource> source, ScrimaQueryOptions<TSource> scrimaQueryOptions,
            Expression<Func<TSource, string, bool>> searchPredicate)
        {
            // 1. sort to have the correct order for filtering and limiting
            source = source.OrderBy(scrimaQueryOptions.OrderBy);

            // 2. filter the items according to the user input
            source = source.Where(scrimaQueryOptions.Filter);

            // 3. handle the "search" parameter
            return source.Search(scrimaQueryOptions, searchPredicate);
        }

        private static IQueryable<TSource> Search<TSource>(this IQueryable<TSource> source, ScrimaQueryOptions scrimaQueryOptions, Expression<Func<TSource, string, bool>> searchPredicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (scrimaQueryOptions == null) throw new ArgumentNullException(nameof(scrimaQueryOptions));

            if (scrimaQueryOptions.Search is null || searchPredicate is null)
                return source;
            
            // we are transforming a generic expression with two arguments into
            // a single argument expression that uses a constant value instead of a parameter
            // e.g.:
            // original:
            //                (element, searchText) => element.MyValue.Contains(searchText)
            // transformed:
            //                var x = "my current search text";
            //                element => element.MyValue.Contains(x);
            var predicate = ExpressionHelper.BindSecondArgument(searchPredicate, scrimaQueryOptions.Search);
            return source.Where(predicate);
        }

        public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> source,
            ScrimaQueryOptions scrimaQueryOptions)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (scrimaQueryOptions == null) throw new ArgumentNullException(nameof(scrimaQueryOptions));

            source = source.Skip((int)(scrimaQueryOptions.Skip ?? 0));

            var top = scrimaQueryOptions.Top;
            if (top.HasValue)
            {
                source = source.Take((int)top);
            }

            return source;
        }
    }
}
