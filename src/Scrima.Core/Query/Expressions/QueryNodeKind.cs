namespace Scrima.Core.Query.Expressions
{
    /// <summary>
    /// Gets the kinds of query node
    /// </summary>
    public enum QueryNodeKind
    {
        /// <summary>
        /// The query node kind is not specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The query node is a binary operator query node.
        /// </summary>
        BinaryOperator = 1,

        /// <summary>
        /// The query node is a property access query node.
        /// </summary>
        PropertyAccess = 2,

        /// <summary>
        /// The query node is a constant value query node.
        /// </summary>
        Constant = 3,

        /// <summary>
        /// The query node is a function call query node.
        /// </summary>
        FunctionCall = 4,

        /// <summary>
        /// The query node is a unary operator query node.
        /// </summary>
        UnaryOperator = 5,
        
        /// <summary>
        /// The query node is an array of constant values query node.
        /// </summary>
        Array = 6,
    }
}
