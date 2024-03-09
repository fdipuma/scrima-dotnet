namespace Scrima.Core.Model;

/// <summary>
/// A class which represents an enum member in the Entity Data Model.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{Value}: {Name}")]
public sealed class EdmEnumMember
{
    public EdmEnumMember(string name, int value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Gets the name of the enum member.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the integer value of the enum member.
    /// </summary>
    public int Value { get; }
}