using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Scrima.Core.Query
{
  /// <summary>
    /// A class containing deserialised values from the $orderby query option.
    /// </summary>
    public sealed class OrderByQueryOption 
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="OrderByQueryOption"/> class.
        /// </summary>
        public OrderByQueryOption(IEnumerable<OrderByProperty> properties)
        {
            Properties = new ReadOnlyCollection<OrderByProperty>(properties.ToList());
        }

        /// <summary>
        /// Gets the properties the query should be ordered by.
        /// </summary>
        public IReadOnlyList<OrderByProperty> Properties { get; }

        public override string ToString()
        {
            if (Properties.Count == 0)
                return "OrderBy=<none>";

            var props = string.Join(",", Properties);
            
            return $"OrderBy={props}";
        }
    }
}
