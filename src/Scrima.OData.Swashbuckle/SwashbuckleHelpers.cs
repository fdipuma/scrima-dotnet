using System;
using Scrima.Core.Query;

namespace Scrima.OData.Swashbuckle;

internal static class SwashbuckleHelpers
{
    private static readonly Type OdataOptionsType = typeof(Scrima.OData.AspNetCore.ODataQuery<>);
    private static readonly Type OdataBaseOptionsType = typeof(Scrima.OData.AspNetCore.ODataQuery);
        
    public static bool IsODataQuery(this Type type)
    {
        if (type is null) return false;
        
        return OdataBaseOptionsType.IsAssignableFrom(type) || (type.IsGenericType && type.GetGenericTypeDefinition() == OdataOptionsType);
    }

    public static Type GetScrimaQueryOptionsItemType(this Type type)
    {
        if (type is null) return null;
        
        if (type.IsGenericType && type.GetGenericTypeDefinition() == OdataOptionsType)
        {
            return type.GetGenericArguments()[0];
        }

        return null;
    }
}
