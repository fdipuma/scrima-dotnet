using System;
using System.Collections.Generic;
using Scrima.Core.Query;

namespace Scrima.Queryable
{
    public class ScrimaQueryResult<T>
    {
        public ScrimaQueryResult(ScrimaQueryOptions scrimaQueryOptions, IEnumerable<T> results, long? count)
        {
            ScrimaQueryOptions = scrimaQueryOptions ?? throw new ArgumentNullException(nameof(scrimaQueryOptions));
            Results = results ?? throw new ArgumentNullException(nameof(results));
            Count = count;
        }

        public ScrimaQueryOptions ScrimaQueryOptions { get; }
        public IEnumerable<T> Results { get; }
        public long? Count { get; }
    }
}
