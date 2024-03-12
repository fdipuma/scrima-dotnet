using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Scrima.Core.Model;

namespace Scrima.Core.Query;

/// <summary>
/// A class containing deserialised values from the $orderby query option.
/// </summary>
public sealed class OrderByProperty
{
    public OrderByProperty(EdmProperty property, OrderByDirection direction) : this(new[] {property}, direction)
    {
            
    }
        
    public OrderByProperty(IEnumerable<EdmProperty> properties, OrderByDirection direction)
    {
        if (properties == null) throw new ArgumentNullException(nameof(properties));
            
        Properties = new ReadOnlyCollection<EdmProperty>(properties.ToList());
            
        if (Properties.Count == 0)
        {
            throw new InvalidOperationException("At least one order by property must be selected");
        }
            
        Direction = direction;
    }

    /// <summary>
    /// Gets the direction the property should be ordered by.
    /// </summary>
    public OrderByDirection Direction { get; }

    /// <summary>
    /// Gets the properties to order by.
    /// </summary>
    public IReadOnlyList<EdmProperty> Properties { get; }

    public override string ToString()
    {
        return $"{string.Join(".", Properties)}-{Direction}";
    }
}