using System;

namespace Scrima.Core.Model;

/// <summary>
/// Represents a primitive type in the Entity Data Model.
/// </summary>
/// <seealso cref="EdmType" />
[System.Diagnostics.DebuggerDisplay("{FullName}: {ClrType}")]
public sealed class EdmPrimitiveType : EdmType
{
    private EdmPrimitiveType(string name, string fullName, Type clrType)
        : base(name, fullName, clrType)
    {
    }

    /// <summary>
    /// Gets the EdmType which represent fixed- or variable- length binary data.
    /// </summary>
    public static EdmType Binary { get; } = new EdmPrimitiveType("Binary", "Edm.Binary", typeof(byte[]));

    /// <summary>
    /// Gets the EdmType which represents the mathematical concept of binary-valued logic.
    /// </summary>
    public static EdmType Boolean { get; } = new EdmPrimitiveType("Boolean", "Edm.Boolean", typeof(bool));

    /// <summary>
    /// Gets the EdmType which represents an unsigned 8-bit integer value.
    /// </summary>
    public static EdmType Byte { get; } = new EdmPrimitiveType("Byte", "Edm.Byte", typeof(byte));

    /// <summary>
    /// Gets the EdmType which represents date with values ranging from January 1; 1753 A.D. through December 9999 A.D.
    /// </summary>
    public static EdmType Date { get; } = new EdmPrimitiveType("Date", "Edm.Date", typeof(DateOnly));

    /// <summary>
    /// Gets the EdmType which represents date and time as an Offset in minutes from GMT; with values ranging from 12:00:00 midnight; January 1; 1753 A.D. through 11:59:59 P.M; December 9999 A.D.
    /// </summary>
    public static EdmType DateTimeOffset { get; } =
        new EdmPrimitiveType("DateTimeOffset", "Edm.DateTimeOffset", typeof(DateTimeOffset));

    /// <summary>
    /// Gets the EdmType which represents numeric values with fixed precision and scale. This type can describe a numeric value ranging from negative 10^255 + 1 to positive 10^255 -1
    /// </summary>
    public static EdmType Decimal { get; } = new EdmPrimitiveType("Decimal", "Edm.Decimal", typeof(decimal));

    /// <summary>
    /// Gets the EdmType which represents a floating point number with 15 digits precision that can represent values with approximate range of ± 2.23e -308 through ± 1.79e +308
    /// </summary>
    public static EdmType Double { get; } = new EdmPrimitiveType("Double", "Edm.Double", typeof(double));

    /// <summary>
    /// Gets the EdmType which represents a duration.
    /// </summary>
    public static EdmType Duration { get; } = new EdmPrimitiveType("Duration", "Edm.Duration", typeof(TimeSpan));

    /// <summary>
    /// Gets the EdmType which represents a 16-byte (128-bit) unique identifier value.
    /// </summary>
    public static EdmType Guid { get; } = new EdmPrimitiveType("Guid", "Edm.Guid", typeof(Guid));

    /// <summary>
    /// Gets the EdmType which represents a signed 16-bit integer value.
    /// </summary>
    public static EdmType Int16 { get; } = new EdmPrimitiveType("Int16", "Edm.Int16", typeof(short));

    /// <summary>
    /// Gets the EdmType which represents a signed 32-bit integer value.
    /// </summary>
    public static EdmType Int32 { get; } = new EdmPrimitiveType("Int32", "Edm.Int32", typeof(int));

    /// <summary>
    /// Gets the EdmType which represents a signed 64-bit integer value.
    /// </summary>
    public static EdmType Int64 { get; } = new EdmPrimitiveType("Int64", "Edm.Int64", typeof(long));

    /// <summary>
    /// Gets the EdmType which represents a signed 8-bit integer value.
    /// </summary>
    public static EdmType SByte { get; } = new EdmPrimitiveType("SByte", "Edm.SByte", typeof(sbyte));

    /// <summary>
    /// Gets the EdmType which represents a floating point number with 7 digits precision that can represent values with approximate range of ± 1.18e -38 through ± 3.40e +38
    /// </summary>
    public static EdmType Single { get; } = new EdmPrimitiveType("Single", "Edm.Single", typeof(float));

    /// <summary>
    /// Gets the EdmType which represents fixed- or variable-length character data.
    /// </summary>
    public static EdmType String { get; } = new EdmPrimitiveType("String", "Edm.String", typeof(string));

    /// <summary>
    /// Gets the EdmType which represents the time of day with values ranging from 0:00:00.x to 23:59:59.y, where x and y depend upon the precision.
    /// </summary>
    public static EdmType TimeOfDay { get; } = new EdmPrimitiveType("TimeOfDay", "Edm.TimeOfDay", typeof(TimeOnly));

    public override int GetHashCode() => Name.GetHashCode();
}
