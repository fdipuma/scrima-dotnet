namespace Scrima.Core.Query.Expressions
{
    /// <summary>
    /// The base class for a query node.
    /// </summary>
    public abstract class QueryNode
    {
        /// <summary>
        /// Gets the kind of query node.
        /// </summary>
        public abstract QueryNodeKind Kind { get; }

        public override string ToString() => Kind.ToString();
    }
}
