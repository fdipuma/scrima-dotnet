using System;
using Scrima.Core.Query;

namespace Scrima.OData.Swashbuckle
{
    internal static class SwashbuckleHelpers
    {
        private static readonly Type BaseOptionsType = typeof(ScrimaQueryOptions);
        private static readonly Type GenericOptionsType = typeof(QueryOptions<>);
        
        public static bool IsScrimaQueryOptions(this Type type)
        {
            return BaseOptionsType.IsAssignableFrom(type);
        }

        public static Type GetScrimaQueryOptionsItemType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == GenericOptionsType)
            {
                return type.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
