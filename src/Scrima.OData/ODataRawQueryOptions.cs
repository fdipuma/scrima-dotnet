using System;
using System.Text;
using Scrima.OData.Parsers;

namespace Scrima.OData
{
    /// <summary>
    /// A class which contains the raw request values.
    /// </summary>
    public sealed class ODataRawQueryOptions
    {
        public const string FilterParamName = "$filter";
        public const string OrderByParamName = "$orderby";
        public const string SkipParamName = "$skip";
        public const string TopParamName = "$top";
        public const string SearchParamName = "$search";
        public const string SkipTokenParamName = "$skiptoken";
        public const string CountParamName = "$count";

        /// <summary>
        /// Initialises a new instance of the <see cref="ODataRawQueryOptions"/> class.
        /// </summary>
        /// <param name="rawQuery">The raw query.</param>
        /// <exception cref="ArgumentNullException">Thrown if raw query is null.</exception>
        public static ODataRawQueryOptions ParseRawQuery(string rawQuery)
        {
            const string filterFullParam = FilterParamName + "=";
            const string orderByFullParam = OrderByParamName + "=";
            const string skipFullParam = SkipParamName + "=";
            const string topFullParam = TopParamName + "=";
            const string searchFullParam = SearchParamName + "=";
            const string skipTokenFullParam = SkipTokenParamName + "=";
            const string countFullParam = CountParamName + "=";

            var options = new ODataRawQueryOptions();

            if (rawQuery == null)
            {
                throw new ArgumentNullException(nameof(rawQuery));
            }

            // Any + signs we want in the data should have been encoded as %2B,
            // so do the replace first otherwise we replace legitemate + signs!
            rawQuery = rawQuery.Replace('+', ' ');

            if (rawQuery.Length > 0)
            {
                // Drop the ?
                var query = rawQuery.StartsWith("?") ? rawQuery.Substring(1) : rawQuery;

                var queryOptions = query.Split(SplitCharacter.Ampersand, StringSplitOptions.RemoveEmptyEntries);

                foreach (var queryOption in queryOptions)
                {
                    // Decode the chunks to prevent splitting the query on an '&' which is actually part of a string value
                    var rawQueryOption = Uri.UnescapeDataString(queryOption);

                    if (rawQueryOption.StartsWith(filterFullParam, StringComparison.Ordinal))
                    {
                        if (rawQueryOption.Length != filterFullParam.Length)
                        {
                            options.Filter = rawQueryOption.Substring(filterFullParam.Length);
                        }
                    }
                    else if (rawQueryOption.StartsWith(orderByFullParam, StringComparison.Ordinal))
                    {
                        if (rawQueryOption.Length != orderByFullParam.Length)
                        {
                            options.OrderBy = rawQueryOption.Substring(orderByFullParam.Length);
                        }
                    }
                    else if (rawQueryOption.StartsWith(skipFullParam, StringComparison.Ordinal))
                    {
                        if (rawQueryOption.Length != skipFullParam.Length)
                        {
                            options.Skip = rawQueryOption.Substring(skipFullParam.Length);
                        }
                    }
                    else if (rawQueryOption.StartsWith(topFullParam, StringComparison.Ordinal))
                    {
                        if (rawQueryOption.Length != topFullParam.Length)
                        {
                            options.Top = rawQueryOption.Substring(topFullParam.Length);
                        }
                    }
                    else if (rawQueryOption.StartsWith(searchFullParam, StringComparison.Ordinal))
                    {
                        if (rawQueryOption.Length != searchFullParam.Length)
                        {
                            options.Search = rawQueryOption.Substring(searchFullParam.Length);
                        }
                    }
                    else if (rawQueryOption.StartsWith(skipTokenFullParam, StringComparison.Ordinal))
                    {
                        if (rawQueryOption.Length != skipTokenFullParam.Length)
                        {
                            options.SkipToken = rawQueryOption.Substring(skipTokenFullParam.Length);
                        }
                    }
                    else if (rawQueryOption.StartsWith(countFullParam, StringComparison.Ordinal))
                    {
                        if (rawQueryOption.Length != countFullParam.Length)
                        {
                            options.Count = rawQueryOption.Substring(countFullParam.Length);
                        }
                    }
                }
            }

            return options;
        }

        /// <summary>
        /// Gets the raw $count query value from the incoming request Uri if specified.
        /// </summary>
        public string Count { get; set; }

        /// <summary>
        /// Gets the raw $filter query value from the incoming request Uri if specified.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets the raw $orderby query value from the incoming request Uri if specified.
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets the raw $search query value from the incoming request Uri if specified.
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Gets the raw $skip query value from the incoming request Uri if specified.
        /// </summary>
        public string Skip { get; set; }

        /// <summary>
        /// Gets the raw $skip token query value from the incoming request Uri if specified.
        /// </summary>
        public string SkipToken { get; set; }

        /// <summary>
        /// Gets the raw $top query value from the incoming request Uri if specified.
        /// </summary>
        public string Top { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            
            if (Filter != null)
            {
                builder.Append(FilterParamName);
                builder.Append('=');
                builder.Append(Filter);
                builder.Append('&');
            }

            if (OrderBy != null)
            {
                builder.Append(OrderByParamName);
                builder.Append('=');
                builder.Append(OrderBy);
                builder.Append('&');
            }

            if (Skip != null)
            {
                builder.Append(SkipParamName);
                builder.Append('=');
                builder.Append(Skip);
                builder.Append('&');
            }

            if (Top != null)
            {
                builder.Append(TopParamName);
                builder.Append('=');
                builder.Append(Top);
                builder.Append('&');
            }

            if (Search != null)
            {
                builder.Append(SearchParamName);
                builder.Append('=');
                builder.Append(Search);
                builder.Append('&');
            }

            if (SkipToken != null)
            {
                builder.Append(SkipTokenParamName);
                builder.Append('=');
                builder.Append(SkipToken);
                builder.Append('&');
            }

            if (Count != null)
            {
                builder.Append(CountParamName);
                builder.Append('=');
                builder.Append(Count);
                builder.Append('&');
            }

            return builder.ToString(0, builder.Length > 0 ? builder.Length - 1 : builder.Length);
        }
    }
}
