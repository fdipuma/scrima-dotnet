using System;

namespace Scrima.OData.AspNetCore
{
    internal static class TypeExtensions
    {
        public static bool IsODataQuery(this Type bindingContextModelType)
        {
            if (bindingContextModelType is null) return false;
            
            return 
                bindingContextModelType.IsGenericType &&
                bindingContextModelType.GetGenericTypeDefinition() == typeof(ODataQuery<>);
        }
    }
}
