using System;
using System.Collections.Generic;

namespace Scrima.Core.Model;

/// <summary>
/// Represents a complex type in the Entity Data Model.
/// </summary>
/// <seealso cref="EdmType" />
[System.Diagnostics.DebuggerDisplay("{FullName}: {ClrType}")]
public sealed class EdmComplexType : EdmType
{
    public EdmComplexType(Type clrType, IReadOnlyList<EdmProperty> properties)
        : this(clrType, null, properties)
    {
    }

    public EdmComplexType(Type clrType, EdmType baseType, IReadOnlyList<EdmProperty> properties)
        : base(clrType.Name, clrType.FullName, clrType)
    {
        BaseType = baseType;
        Properties = properties ?? throw new ArgumentNullException(nameof(properties));
    }

    /// <summary>
    /// Gets the <see cref="EdmType"/> from which the current <see cref="EdmComplexType"/> directly inherits.
    /// </summary>
    public EdmType BaseType { get; }

    /// <summary>
    /// Gets the properties defined on the type.
    /// </summary>
    public IReadOnlyList<EdmProperty> Properties { get; }

    /// <summary>
    /// Gets the property with the specified name.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <returns>The <see cref="EdmProperty"/> declared in this type with the specified name.</returns>
    /// <exception cref="System.ArgumentException">The type does not contain a property with the specified name.</exception>
    public EdmProperty GetProperty(string name)
    {
        if (TryGetProperty(name, out var property))
        {
            return property;
        }

        throw new ArgumentException($"The type '{FullName}' does not contain a property named '{name}'");
    }

    /// <summary>
    /// Gets the property with the specified name.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="property">The <see cref="EdmProperty"/> declared in this type with the specified name, if any.</param>
    /// <returns><c>true</c> if the type contains a property with the specified name; otherwise, <c>false</c>.</returns>
    public bool TryGetProperty(string name, out EdmProperty property)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        foreach (var candidate in Properties)
        {
            if (string.Equals(name, candidate.Name, StringComparison.OrdinalIgnoreCase))
            {
                property = candidate;
                return true;
            }
        }

        property = null;
        return false;
    }
}