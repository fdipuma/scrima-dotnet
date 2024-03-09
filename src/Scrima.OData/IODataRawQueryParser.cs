using System;
using Scrima.Core.Query;

namespace Scrima.OData;

public interface IODataRawQueryParser
{
    QueryOptions ParseOptions(Type itemType, ODataRawQueryOptions rawQuery, ODataQueryDefaultOptions defaultOptions = null);
}