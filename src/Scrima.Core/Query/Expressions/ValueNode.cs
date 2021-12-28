using Scrima.Core.Model;

namespace Scrima.Core.Query.Expressions
{
    /// <summary>
    /// A QueryNode which represents a value (property or constant).
    /// </summary>
    public abstract class ValueNode : QueryNode
    {
        /// <summary>
        /// Gets the <see cref="EdmType"/> of the value node.
        /// </summary>
        public abstract EdmType EdmValueType { get; }

        public override string ToString() => EdmValueType?.Name;
    }
}
