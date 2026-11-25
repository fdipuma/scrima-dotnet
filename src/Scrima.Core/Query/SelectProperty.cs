using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Scrima.Core.Model;

namespace Scrima.Core.Query;

/// <summary>
/// A property selected by the $select query option.
/// </summary>
public sealed class SelectProperty
{
    public SelectProperty(EdmProperty property) : this(new[] { property })
    {
    }

    public SelectProperty(IEnumerable<EdmProperty> properties)
    {
        if (properties == null) throw new ArgumentNullException(nameof(properties));

        Properties = new ReadOnlyCollection<EdmProperty>(properties.ToList());

        if (Properties.Count == 0)
        {
            throw new ArgumentException("At least one property must be selected", nameof(properties));
        }
    }

    /// <summary>
    /// Gets the path to the selected property, starting from a property of the queried type.
    /// </summary>
    public IReadOnlyList<EdmProperty> Properties { get; }

    public override string ToString() => string.Join(".", Properties);
}
