using System;
using Scrima.Core.Model;

namespace Scrima.Core.Query
{
    /// <summary>
    /// A class containing deserialised values from the $orderby query option.
    /// </summary>
    public sealed class OrderByProperty
    {
        public OrderByProperty(EdmProperty property, OrderByDirection direction)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Direction = direction;
        }

        /// <summary>
        /// Gets the direction the property should be ordered by.
        /// </summary>
        public OrderByDirection Direction { get; }

        /// <summary>
        /// Gets the property to order by.
        /// </summary>
        public EdmProperty Property { get; }

        public override string ToString() => $"{Property.Name}-{Direction}";
    }
}
