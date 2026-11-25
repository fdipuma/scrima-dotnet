using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Scrima.Core.Exceptions;
using Scrima.Core.Model;
using Scrima.Core.Query;

namespace Scrima.Queryable;

public static partial class ScrimaExtensions
{
    /// <summary>
    /// Projects each element of a sequence into a new instance of the same type,
    /// populating only the properties selected by the provided <see cref="SelectQueryOption"/>
    /// </summary>
    /// <param name="source">The queryable source</param>
    /// <param name="selectQueryOption">The select options to apply</param>
    /// <returns>The projected data source</returns>
    /// <exception cref="ArgumentNullException">source or select are null</exception>
    /// <exception cref="QueryOptionsValidationException">A selected property cannot be set</exception>
    public static IQueryable<TSource> Select<TSource>(this IQueryable<TSource> source,
        SelectQueryOption selectQueryOption)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selectQueryOption == null) throw new ArgumentNullException(nameof(selectQueryOption));

        if (selectQueryOption.Properties.Count == 0) return source;

        var parameter = Expression.Parameter(typeof(TSource), "o");

        var paths = selectQueryOption.Properties.Select(p => p.Properties).ToList();

        var projection = CreateProjection(parameter, paths);

        return source.Select(Expression.Lambda<Func<TSource, TSource>>(projection, parameter));
    }

    /// <summary>
    /// Creates a new instance of the source type with only the selected properties populated, e.g.:
    /// <c>new Post { Title = o.Title, Blog = o.Blog == null ? null : new Blog { Name = o.Blog.Name } }</c>
    /// </summary>
    private static MemberInitExpression CreateProjection(Expression source,
        IReadOnlyCollection<IReadOnlyList<EdmProperty>> paths)
    {
        var bindings = paths
            .GroupBy(path => path[0].Name)
            .Select(group => CreateBinding(source, group.Key, group.ToList()));

        return Expression.MemberInit(Expression.New(source.Type), bindings);
    }

    private static MemberBinding CreateBinding(Expression source, string propertyName,
        IReadOnlyCollection<IReadOnlyList<EdmProperty>> paths)
    {
        var property = source.Type.GetProperty(propertyName);

        if (property is not { CanWrite: true })
        {
            throw new QueryOptionsValidationException($"Property {propertyName} cannot be selected");
        }

        var value = Expression.Property(source, property);

        // selecting a whole property takes precedence over selecting some of its nested properties
        if (paths.Any(path => path.Count == 1))
        {
            return Expression.Bind(property, value);
        }

        var nestedPaths = paths.Select(path => path.Skip(1).ToList()).ToList();
        var nullValue = Expression.Constant(null, value.Type);

        var nestedProjection = Expression.Condition(
            Expression.Equal(value, nullValue),
            nullValue,
            CreateProjection(value, nestedPaths));

        return Expression.Bind(property, nestedProjection);
    }
}
