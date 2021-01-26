using System;
using System.Linq;
using Scrima.Core;
using Scrima.Core.Model;
using Scrima.Core.Query;

namespace Scrima.OData
{
    public class ODataRawRawQueryParser : IODataRawQueryParser
    {
        private readonly EdmTypeProvider _typeProvider;

        public ODataRawRawQueryParser()
        {
            _typeProvider = new EdmTypeProvider();
        }
        
        public QueryOptions ParseOptions(Type itemType, ODataRawQueryOptions rawQuery, ODataQueryDefaultOptions defaultOptions = null)
        {
            var options = ParseInternal(itemType, rawQuery, defaultOptions);

            return Activator.CreateInstance(typeof(QueryOptions<>).MakeGenericType(itemType), options) as QueryOptions;
        }

        public QueryOptions<T> ParseOptions<T>(ODataRawQueryOptions rawQuery, ODataQueryDefaultOptions defaultOptions = null)
        {
            var options = ParseInternal(typeof(T), rawQuery, defaultOptions);

            return new QueryOptions<T>(options);
        }

        private QueryOptions ParseInternal(Type itemType, ODataRawQueryOptions rawQuery, ODataQueryDefaultOptions defaultOptions)
        {
            if (itemType == null) throw new ArgumentNullException(nameof(itemType));
            if (rawQuery == null) throw new ArgumentNullException(nameof(rawQuery));

            var modelType = (EdmComplexType) _typeProvider.GetByClrType(itemType);

            var filterOptions = rawQuery.Filter != null
                ? ODataQueryParseHelper.ParseFilter(rawQuery.Filter, modelType, _typeProvider)
                : new FilterQueryOption(null);

            var orderByOptions = rawQuery.OrderBy != null
                ? ODataQueryParseHelper.ParseOrderBy(rawQuery.OrderBy, modelType)
                : new OrderByQueryOption(Enumerable.Empty<OrderByProperty>());

            var searchOptions = rawQuery.Search;

            var skipOption = rawQuery.Skip != null
                ? long.TryParse(rawQuery.Skip, out var skipValue) ? skipValue :
                throw new ODataParseException("Invalid value for $skip")
                : (long?) null;

            var skipTokenOptions = rawQuery.SkipToken;

            var topOption = rawQuery.Top != null
                ? long.TryParse(rawQuery.Top, out var topValue) ? topValue :
                throw new ODataParseException("Invalid value for $top")
                : (long?) null;

            if (defaultOptions?.DefaultTop is not null && topOption is null)
                topOption = defaultOptions.DefaultTop;

            if (defaultOptions?.MaxTop is not null && topOption > defaultOptions.MaxTop)
                topOption = defaultOptions.MaxTop;

            var countOption = false;

            if (rawQuery.Count != null)
            {
                if (!bool.TryParse(rawQuery.Count, out countOption))
                    throw new ODataParseException("Invalid value for $count");
            }

            if (defaultOptions?.AlwaysShowCount == true)
                countOption = true;

            return new QueryOptions(
                modelType,
                filterOptions,
                orderByOptions,
                searchOptions,
                skipOption,
                skipTokenOptions,
                topOption,
                countOption);
        }
    }
}
