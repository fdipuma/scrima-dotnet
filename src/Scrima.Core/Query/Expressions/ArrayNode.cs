using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Scrima.Core.Model;

namespace Scrima.Core.Query.Expressions
{
    /// <summary>
    /// A QueryNode which represents an array constant values.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{LiteralText}")]
    public sealed class ArrayNode : ValueNode
    {
        private readonly List<ConstantNode> _elements;

        public IReadOnlyList<ConstantNode> Elements => new ReadOnlyCollection<ConstantNode>(_elements);

        public ArrayNode(IEnumerable<ConstantNode> elements)
        {
            _elements = elements.ToList();

            if (_elements.Select(t => t.EdmType).Distinct().Count() > 1)
            {
                throw new InvalidOperationException("Unable to create an array node of distinct constant values");
            }
        }

        public override QueryNodeKind Kind => QueryNodeKind.Array;
        public override EdmType EdmValueType => _elements[0].EdmValueType;

        public Type ArrayClrType => EdmValueType.ClrType.MakeArrayType();
        
        public string LiteralText => $"({string.Join(",", _elements.Select(e => e.LiteralText))})";

        public void AddElement(ConstantNode element)
        {
            if (element.EdmType != EdmValueType)
            {
                throw new InvalidOperationException($"Unable to add element of type {element.EdmValueType} to array of type {EdmValueType.Name}");
            }

            _elements.Add(element);
        }

        /// <summary>
        /// Gets the constant value as an object.
        /// </summary>
        public object Value
        {
            get
            {
                var instance = Array.CreateInstance(EdmValueType.ClrType, _elements.Count);
                for (var i = 0; i < _elements.Count; i++)
                {
                    var element = _elements[i];
                    instance.SetValue(element.Value, i);
                }

                return instance;
            }
        }

        public override string ToString() => LiteralText;
    }
}
