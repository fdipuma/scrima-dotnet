using System;
using Scrima.Core.Query;

namespace Scrima.OData.AspNetCore
{
    internal static class TypeExtensions
    {
        public static bool IsQueryOptions(this Type bindingContextModelType)
        {
            var isQueryOptions =
                bindingContextModelType.IsGenericType &&
                bindingContextModelType.GetGenericTypeDefinition() == typeof(ScrimaQueryOptions<>);
            
            return isQueryOptions;
        }
    }
}