using System.Collections.Generic;

namespace Scrima.Core.Query.Expressions
{
    /// <summary>
    /// A QueryNode which represents a function call.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class FunctionCallNode : QueryNode
    {
        private readonly List<QueryNode> _parameters = new List<QueryNode>();

        /// <summary>
        /// Initialises a new instance of the <see cref="FunctionCallNode" /> class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        public FunctionCallNode(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the kind of query node.
        /// </summary>
        public override QueryNodeKind Kind => QueryNodeKind.FunctionCall;

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the parameters for the function call.
        /// </summary>
        public IReadOnlyList<QueryNode> Parameters => _parameters;

        public void AddParameter(QueryNode queryNode) =>_parameters.Add(queryNode);
    }
}