using System;
using System.Linq;
using System.Linq.Expressions;
using Scrima.Core.Exceptions;
using Scrima.Core.Query;

namespace Scrima.Queryable;

public static partial class ScrimaExtensions
{
    /// <summary>
    /// Projects each element of a sequence into a new instance of the same type,
    /// populating only the properties selected by the provided <see cref="SelectQueryOption"/>
    /// </summary>
    /// <remarks>
    /// Instances of types derived from <typeparamref name="TSource"/> are returned as <typeparamref name="TSource"/>.
    /// </remarks>
    /// <param name="source">The queryable source</param>
    /// <param name="selectQueryOption">The select options to apply</param>
    /// <returns>The projected data source</returns>
    /// <exception cref="ArgumentNullException">source or select are null</exception>
    /// <exception cref="QueryOptionsValidationException">A selected property cannot be set, or a type cannot be created</exception>
    public static IQueryable<TSource> Select<TSource>(this IQueryable<TSource> source,
        SelectQueryOption selectQueryOption)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selectQueryOption == null) throw new ArgumentNullException(nameof(selectQueryOption));

        var projection = selectQueryOption.CreateProjection(typeof(TSource));

        if (projection == null) return source;

        return source.Select((Expression<Func<TSource, TSource>>)projection);
    }
}
