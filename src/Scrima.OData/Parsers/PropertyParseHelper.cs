using System.Collections.Generic;
using Scrima.Core.Model;

namespace Scrima.OData.Parsers;

internal static class PropertyParseHelper
{
    public static IEnumerable<EdmProperty> ParseNestedProperties(string tokenValue, EdmComplexType edmComplexType)
    {
        var properties = new List<EdmProperty>();

        foreach (var propertyName in tokenValue.Split('/'))
        {
            if (edmComplexType is null || !edmComplexType.TryGetProperty(propertyName, out var currentProperty))
            {
                throw new ODataParseException($"Property {propertyName} not found");
            }

            properties.Add(currentProperty);
            edmComplexType = currentProperty.PropertyType as EdmComplexType;
        }

        return properties;
    }
}
