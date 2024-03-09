using System;

namespace Scrima.Core.Model;

/// <summary>
/// Represents a type in the Entity Data Model.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{FullName}: {ClrType}")]
public abstract class EdmType : IEquatable<EdmType>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="EdmType"/> class.
    /// </summary>
    /// <param name="name">The name of the type.</param>
    /// <param name="fullName">The full name of the type.</param>
    /// <param name="clrType">The CLR type.</param>
    protected EdmType(string name, string fullName, Type clrType)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name must be specified", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("FullName must be specified", nameof(fullName));
        }

        Name = name;
        FullName = fullName;
        ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
    }

    /// <summary>
    /// Gets the CLR type.
    /// </summary>
    public Type ClrType { get; }

    /// <summary>
    /// Gets the full name.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj) => Equals(obj as EdmType);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals(EdmType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ClrType == other.ClrType;
    }

    public static bool operator ==(EdmType a, EdmType b)
    {
        if (a is null) return b is null;

        if (b is null) return false;
            
        if (a.GetType() != b.GetType()) return false;

        return a.Equals(b);
    }

    public static bool operator !=(EdmType a, EdmType b)
    {
        if (a is null) return b is not null;
            
        if (b is null) return true;
            
        if (a.GetType() != b.GetType()) return true;

        return !a.Equals(b);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode() => ClrType.GetHashCode();

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString() => FullName;
}