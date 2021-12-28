namespace Scrima.Core.Query.Expressions
{
    /// <summary>
    /// A QueryNode which represents a unary operator.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{OperatorKind} {Operand}")]
    public sealed class UnaryOperatorNode : QueryNode
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UnaryOperatorNode"/> class.
        /// </summary>
        /// <param name="operand">The operand of the unary operator.</param>
        /// <param name="operatorKind">Kind of the operator.</param>
        public UnaryOperatorNode(QueryNode operand, UnaryOperatorKind operatorKind)
        {
            Operand = operand;
            OperatorKind = operatorKind;
        }

        /// <summary>
        /// Gets the kind of query node.
        /// </summary>
        public override QueryNodeKind Kind => QueryNodeKind.UnaryOperator;

        /// <summary>
        /// Gets the operand of the unary operator.
        /// </summary>
        public QueryNode Operand { get; }

        /// <summary>
        /// Gets the kind of the operator.
        /// </summary>
        public UnaryOperatorKind OperatorKind { get; }

        public override string ToString() => $"{OperatorKind} {Operand}";
    }
}
