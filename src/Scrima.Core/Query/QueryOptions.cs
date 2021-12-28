using System;
using System.Text;
using Scrima.Core.Model;

namespace Scrima.Core.Query
{
    /// <summary>
    /// An object which contains query options
    /// </summary>
    public class QueryOptions
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="QueryOptions" /> class.
        /// </summary>
        /// <param name="edmType"></param>
        /// <param name="filter">Filter query option</param>
        /// <param name="orderBy">Order by query option</param>
        /// <param name="search">Search query option</param>
        /// <param name="skip">Skip query option</param>
        /// <param name="skipToken">Skip token query option</param>
        /// <param name="top">Top query option</param>
        /// <param name="showCount">Show count query option</param>
        public QueryOptions(EdmComplexType edmType, FilterQueryOption filter, OrderByQueryOption orderBy,
            string search, long? skip, string skipToken, long? top, bool showCount)
        {
            EdmType = edmType ?? throw new ArgumentNullException(nameof(edmType));
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
            OrderBy = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
            Search = search;
            Skip = skip;
            SkipToken = skipToken;
            Top = top;
            ShowCount = showCount;
        }

        /// <summary>
        /// Gets the model type
        /// </summary>
        public EdmComplexType EdmType { get; }

        /// <summary>
        /// Gets the filter query option.
        /// </summary>
        public FilterQueryOption Filter { get; }

        /// <summary>
        /// Gets the order by query option.
        /// </summary>
        public OrderByQueryOption OrderBy { get; }

        /// <summary>
        /// Gets the search query option.
        /// </summary>
        public string Search { get; }

        /// <summary>
        /// Gets the skip query option.
        /// </summary>
        public long? Skip { get; }

        /// <summary>
        /// Gets the skip token query option.
        /// </summary>
        public string SkipToken { get; }

        /// <summary>
        /// Gets the top query option.
        /// </summary>
        public long? Top { get; }

        /// <summary>
        /// Gets the show count query option.
        /// </summary>
        public bool ShowCount { get; }

        public override string ToString()
        {
            var builder = new StringBuilder($"QueryOptions[{EdmType.Name}]: ");

            if (Filter is not null)
            {
                builder.Append($"{Filter}; ");
            }

            if (Search is not null)
            {
                builder.Append($"Search={Search}; ");
            }

            if (Skip is not null)
            {
                builder.Append($"Skip={Skip}; ");
            }

            if (Top is not null)
            {
                builder.Append($"Top={Top}; ");
            }

            if (SkipToken is not null)
            {
                builder.Append($"SkipToken={SkipToken}; ");
            }

            if (OrderBy is not null)
            {
                builder.Append($"{OrderBy}; ");
            }
            
            builder.Append($"ShowCount={ShowCount}");

            return builder.ToString();
        }
    }
}
