using System;
using Scrima.Core.Query;

namespace Scrima.OData.AspNetCore
{
    internal static class TypeExtensions
    {
        public static bool IsODataQuery(this Type bindingContextModelType)
        {
            var isQueryOptions =
                bindingContextModelType.IsGenericType &&
                bindingContextModelType.GetGenericTypeDefinition() == typeof(ODataQuery<>);
            
            return isQueryOptions;
        }
    }
}
