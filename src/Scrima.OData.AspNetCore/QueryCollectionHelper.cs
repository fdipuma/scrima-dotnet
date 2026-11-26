using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Scrima.OData.AspNetCore;

internal static class QueryCollectionHelper
{
    public static ODataRawQueryOptions CreateODataRawQueryOptions(IQueryCollection query)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var options = new ODataRawQueryOptions();

        StringValues value;

        // an empty $select is ignored, as in ODataRawQueryOptions.ParseRawQuery
        if (query.TryGetValue(ODataRawQueryOptions.SelectParamName, out value) && !string.IsNullOrEmpty(value.Last()))
        {
            options.Select = value.Last();
        }

        if (query.TryGetValue(ODataRawQueryOptions.FilterParamName, out value))
        {
            options.Filter = value.Last();
        }

        if (query.TryGetValue(ODataRawQueryOptions.OrderByParamName, out value))
        {
            options.OrderBy = value.Last();
        }

        if (query.TryGetValue(ODataRawQueryOptions.SkipParamName, out value))
        {
            options.Skip = value.Last();
        }

        if (query.TryGetValue(ODataRawQueryOptions.TopParamName, out value))
        {
            options.Top = value.Last();
        }

        if (query.TryGetValue(ODataRawQueryOptions.SearchParamName, out value))
        {
            options.Search = value.Last();
        }

        if (query.TryGetValue(ODataRawQueryOptions.SkipTokenParamName, out value))
        {
            options.SkipToken = value.Last();
        }

        if (query.TryGetValue(ODataRawQueryOptions.CountParamName, out value))
        {
            options.Count = value.Last();
        }

        return options;
    }
}
