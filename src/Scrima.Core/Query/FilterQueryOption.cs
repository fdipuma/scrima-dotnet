using Scrima.Core.Query.Expressions;

namespace Scrima.Core.Query
{
    /// <summary>
    /// A class containing deserialised values from the $filter query option.
    /// </summary>
    public sealed class FilterQueryOption
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FilterQueryOption" /> class.
        /// </summary>
        public FilterQueryOption(QueryNode expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public QueryNode Expression { get; }
    }
}