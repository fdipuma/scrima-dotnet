﻿using System;
using Scrima.Core.Query;

namespace Scrima.OData
{
    public interface IODataRawQueryParser
    {
        ScrimaQueryOptions ParseOptions(Type itemType, ODataRawQueryOptions rawQuery, ODataQueryDefaultOptions defaultOptions = null);
        ScrimaQueryOptions<T> ParseOptions<T>(ODataRawQueryOptions rawQuery, ODataQueryDefaultOptions defaultOptions = null);
    }
}