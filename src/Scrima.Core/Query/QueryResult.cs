using System;
using System.Collections.Generic;

namespace Scrima.Core.Query
{
    /// <summary>
    /// Results of a Scrima query
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public class QueryResult<T>
    {
        public QueryResult(IEnumerable<T> results, long? count)
        {
            Results = results ?? throw new ArgumentNullException(nameof(results));
            Count = count;
        }
        
        /// <summary>
        /// Filtered and paginated results of the query
        /// </summary>
        public IEnumerable<T> Results { get; }
        
        /// <summary>
        /// Total count of the filtered items (optional)
        /// </summary>
        public long? Count { get; }
    }
}
