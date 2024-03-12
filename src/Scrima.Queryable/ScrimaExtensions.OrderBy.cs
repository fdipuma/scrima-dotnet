using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Scrima.Core.Exceptions;
using Scrima.Core.Query;

namespace Scrima.Queryable;

public static partial class ScrimaExtensions
{
    /// <summary>
    /// Sorts the elements of a sequence according to the provided <see cref="OrderByQueryOption"/>
    /// </summary>
    /// <param name="source">The original data source</param>
    /// <param name="orderByQueryOption">The options used to order the data source</param>
    /// <returns>The ordered data source</returns>
    /// <exception cref="ArgumentNullException">source or orderByQueryOption are null</exception>
    public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, OrderByQueryOption orderByQueryOption)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (orderByQueryOption == null) throw new ArgumentNullException(nameof(orderByQueryOption));

        if (orderByQueryOption.Properties.Count == 0)
            return source;
            
        // o
        var parameter = Expression.Parameter(source.ElementType, "o");

        var isFirstClause = true;

        foreach (var clause in orderByQueryOption.Properties)
        {
            // o.PropertyName
            Expression propertyExpression = parameter;
                
            foreach (var edmProperty in clause.Properties)
            {
                var property = propertyExpression.Type.GetProperty(edmProperty.Name);
                
                if (property is null)
                {
                    throw new QueryOptionsValidationException($"Invalid property name {edmProperty.Name}");
                }
                
                propertyExpression = Expression.MakeMemberAccess(propertyExpression, property);
            }

            // o => o.PropertyName
            var selectorExpression = Expression.Lambda(propertyExpression, parameter);

            // select correct sort method
            var sortMethodInfo = GetQueryableSortMethodInfo(clause.Direction, isFirstClause);

            var sortMethod =
                sortMethodInfo.MakeGenericMethod(source.ElementType, propertyExpression.Type);

            // create a new query expression which includes the sort call
            var queryExpression = Expression.Call(null, sortMethod, source.Expression, selectorExpression);

            // update source by using the new query expression
            source = source.Provider.CreateQuery<TSource>(queryExpression);

            isFirstClause = false;
        }

        return source;
    }

    private static MethodInfo GetQueryableSortMethodInfo(OrderByDirection orderByDirection, bool isFirstClause)
    {
        return orderByDirection switch
        {
            OrderByDirection.Ascending when isFirstClause => Methods.QueryableOrderBy,
            OrderByDirection.Ascending => Methods.QueryableThenBy,
            OrderByDirection.Descending when isFirstClause => Methods.QueryableOrderByDescending,
            OrderByDirection.Descending => Methods.QueryableThenByDescending,
            _ => throw new QueryOptionsValidationException($"Unsupported order by direction {orderByDirection}")
        };
    }
}
