namespace Scrima.Core.Query.Expressions;

/// <summary>
/// A QueryNode which represents a binary operator with a left and right branch.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{Left} {OperatorKind} {Right}")]
public sealed class BinaryOperatorNode : QueryNode
{
    /// <summary>
    /// Initialises a new instance of the <see cref="BinaryOperatorNode"/> class.
    /// </summary>
    /// <param name="left">The left query node.</param>
    /// <param name="operatorKind">Kind of the operator.</param>
    /// <param name="right">The right query node.</param>
    public BinaryOperatorNode(QueryNode left, BinaryOperatorKind operatorKind, QueryNode right)
    {
        Left = left;
        OperatorKind = operatorKind;
        Right = right;
    }

    /// <summary>
    /// Gets the kind of query node.
    /// </summary>
    public override QueryNodeKind Kind { get; } = QueryNodeKind.BinaryOperator;

    /// <summary>
    /// Gets the left query node.
    /// </summary>
    public QueryNode Left { get; set; }

    /// <summary>
    /// Gets the kind of the operator.
    /// </summary>
    public BinaryOperatorKind OperatorKind { get; }

    /// <summary>
    /// Gets the right query node.
    /// </summary>
    public QueryNode Right { get; set; }

    public override string ToString()
    {
        return $"({Left} {OperatorKind} {Right})";
    }
}