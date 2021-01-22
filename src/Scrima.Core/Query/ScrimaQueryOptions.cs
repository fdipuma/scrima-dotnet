using System;
using Scrima.Core.Model;

namespace Scrima.Core.Query
{
    /// <summary>
    /// An object which contains query options
    /// </summary>
    public class ScrimaQueryOptions
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ScrimaQueryOptions" /> class.
        /// </summary>
        /// <param name="edmType"></param>
        /// <param name="filter">Filter query option</param>
        /// <param name="orderBy">Order by query option</param>
        /// <param name="search">Search query option</param>
        /// <param name="skip">Skip query option</param>
        /// <param name="skipToken">Skip token query option</param>
        /// <param name="top">Top query option</param>
        /// <param name="showCount">Show count query option</param>
        public ScrimaQueryOptions(EdmComplexType edmType, FilterQueryOption filter, OrderByQueryOption orderBy,
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
    }
    
    /// <summary>
    /// An object which contains query options bound to a specific type
    /// </summary>
    public class ScrimaQueryOptions<T> : ScrimaQueryOptions
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ScrimaQueryOptions{T}" /> class.
        /// </summary>
        /// <param name="edmType"></param>
        /// <param name="filter">Filter query option</param>
        /// <param name="orderBy">Order by query option</param>
        /// <param name="search">Search query option</param>
        /// <param name="skip">Skip query option</param>
        /// <param name="skipToken">Skip token query option</param>
        /// <param name="top">Top query option</param>
        /// <param name="showCount"></param>
        public ScrimaQueryOptions(EdmComplexType edmType, FilterQueryOption filter, OrderByQueryOption orderBy, string search,
            long? skip, string skipToken, long? top, bool showCount) : base(edmType, filter, orderBy, search, skip,
            skipToken, top, showCount)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ScrimaQueryOptions{T}" /> class using another <see cref="ScrimaQueryOptions" /> as base .
        /// </summary>
        /// <param name="other">Other options to copy from</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ScrimaQueryOptions(ScrimaQueryOptions other) : this(other.EdmType, other.Filter, other.OrderBy,
            other.Search, other.Skip, other.SkipToken, other.Top, other.ShowCount)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other.EdmType.ClrType != typeof(T))
                throw new ArgumentException("The EdmType of the other query options must be the same of the current type");
        }
    }
}
