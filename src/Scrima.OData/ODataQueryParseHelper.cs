using System;
using System.Linq;
using Scrima.Core;
using Scrima.Core.Exceptions;
using Scrima.Core.Model;
using Scrima.Core.Query;
using Scrima.OData.Parsers;

namespace Scrima.OData;

internal static class ODataQueryParseHelper
{
    public static OrderByQueryOption ParseOrderBy(string rawValue, EdmComplexType model)
    {
        OrderByProperty[] properties;

        if (rawValue.Contains(','))
        {
            properties = rawValue.Split(SplitCharacter.Comma)
                .Select(raw => ParseOrderByProperty(raw, model))
                .ToArray();
        }
        else
        {
            properties = new[]
            {
                ParseOrderByProperty(rawValue, model)
            };
        }

        return new OrderByQueryOption(properties);
    }

    private static OrderByProperty ParseOrderByProperty(string rawValue, EdmComplexType model)
    {
        if (rawValue == null) throw new ArgumentNullException(nameof(rawValue));
        if (model == null) throw new ArgumentNullException(nameof(model));

        var parts = rawValue.Split(SplitCharacter.Space, StringSplitOptions.RemoveEmptyEntries);

        var direction = OrderByDirection.Ascending;

        var propertyName = parts[0];

        var properties = PropertyParseHelper.ParseNestedProperties(propertyName, model);
        
        if (parts.Length != 1)
        {
            switch (parts[1])
            {
                case "asc":
                    direction = OrderByDirection.Ascending;
                    break;

                case "desc":
                    direction = OrderByDirection.Descending;
                    break;

                default:
                    throw new ODataParseException(Messages.OrderByPropertyRawValueInvalid);
            }
        }

        return new OrderByProperty(properties, direction);
    }

    public static FilterQueryOption ParseFilter(string rawQuery, EdmComplexType model, EdmTypeProvider typeProvider)
    {
        if (rawQuery == null) throw new ArgumentNullException(nameof(rawQuery));
        if (model == null) throw new ArgumentNullException(nameof(model));
        
        var filterExpression = FilterExpressionParser.Parse(rawQuery, model, typeProvider);
        return new FilterQueryOption(filterExpression);
    }

    public static SelectQueryOption ParseSelect(string rawValue, EdmComplexType model)
    {
        if (rawValue == null) throw new ArgumentNullException(nameof(rawValue));
        if (model == null) throw new ArgumentNullException(nameof(model));

        var items = rawValue.Split(SplitCharacter.Comma).Select(item => item.Trim()).ToList();

        if (items.Any(item => item.Length == 0))
        {
            throw new ODataParseException(Messages.UnableToParseSelect);
        }

        if (items.Contains("*"))
        {
            return SelectQueryOption.All;
        }

        var properties = items.Select(item => new SelectProperty(PropertyParseHelper.ParseNestedProperties(item, model)));

        var select = new SelectQueryOption(properties);

        // build the projection now, so that an invalid selection is a parse error and not a failure when querying
        try
        {
            select.CreateProjection(model.ClrType);
        }
        catch (QueryOptionsValidationException ex)
        {
            throw new ODataParseException(ex.Message);
        }

        return select;
    }
}
