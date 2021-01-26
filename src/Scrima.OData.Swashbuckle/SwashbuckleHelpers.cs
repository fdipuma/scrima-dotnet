using System;
using Scrima.Core.Query;

namespace Scrima.OData.Swashbuckle
{
    internal static class SwashbuckleHelpers
    {
        private static readonly Type BaseOptionsType = typeof(Scrima.Core.Query.QueryOptions);
        private static readonly Type GenericOptionsType = typeof(Scrima.Core.Query.QueryOptions<>);
        private static readonly Type OdataOptionsType = typeof(Scrima.OData.AspNetCore.ODataQueryParams<>);
        
        public static bool IsScrimaQueryOptions(this Type type)
        {
            return BaseOptionsType.IsAssignableFrom(type) || 
                   (type.IsGenericType && type.GetGenericTypeDefinition() == OdataOptionsType);
        }

        public static Type GetScrimaQueryOptionsItemType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == GenericOptionsType)
            {
                return type.GetGenericArguments()[0];
            }
            
            if (type.IsGenericType && type.GetGenericTypeDefinition() == OdataOptionsType)
            {
                return type.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
