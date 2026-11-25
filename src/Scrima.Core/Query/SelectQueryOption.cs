using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Scrima.Core.Query;

/// <summary>
/// A class containing deserialised values from the $select query option.
/// </summary>
public sealed class SelectQueryOption
{
    /// <summary>
    /// Gets a <see cref="SelectQueryOption"/> which selects all the properties.
    /// </summary>
    public static SelectQueryOption All { get; } = new(Enumerable.Empty<SelectProperty>());

    /// <summary>
    /// Initialises a new instance of the <see cref="SelectQueryOption"/> class.
    /// </summary>
    public SelectQueryOption(IEnumerable<SelectProperty> properties)
    {
        if (properties == null) throw new ArgumentNullException(nameof(properties));

        Properties = new ReadOnlyCollection<SelectProperty>(properties.ToList());
    }

    /// <summary>
    /// Gets the properties to select; when empty, all the properties are selected.
    /// </summary>
    public IReadOnlyList<SelectProperty> Properties { get; }

    public override string ToString() =>
        Properties.Count == 0 ? "Select=*" : $"Select={string.Join(",", Properties)}";
}
